using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using SimpleProductAPI.Configuration;
using SimpleProductAPI.Data;
using SimpleProductAPI.Database;
using SimpleProductAPI.Middleware;
using SimpleProductAPI.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Allow environment variables to override configuration keys.
builder.Configuration.AddEnvironmentVariables();

// Configure Serilog early using configuration.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();



// Resolve connection string from configuration or environment variables.
var connectionString =
    Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? builder.Configuration["ConnectionString"];

if (string.IsNullOrWhiteSpace(connectionString))
{
    // Fail fast with clear message if no connection string is provided.
    throw new InvalidOperationException("Database connection string not found. Set 'ConnectionString', 'ConnectionStrings:Default', or environment variable 'CONNECTION_STRING'.");
}

builder.Services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigOptions>();

builder.Services.AddSingleton<IDbConnectionFactory>(sp =>
    new SqlServerConnectionFactory(
        connectionString,
        sp.GetRequiredService<ILogger<SqlServerConnectionFactory>>()
    ));

builder.Services.AddScoped<IDataProvider, SqlDataProvider>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddApiVersioning( opt =>
{
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
    opt.ReportApiVersions = true;
}).AddMvc()
.AddApiExplorer(opt =>
{
    opt.GroupNameFormat = "'v'VVV";
    opt.SubstituteApiVersionInUrl = true;
    opt.DefaultApiVersion = new ApiVersion(1, 0);
    opt.AssumeDefaultVersionWhenUnspecified = true;
});

try
{
    Log.Information("Starting web host");

    var app = builder.Build();

    // Register correlation id middleware first so subsequent logs include CorrelationId
    app.UseMiddleware<CorrelationIdMiddleware>();

    // Add Serilog request logging so requests/responses are logged with structured properties.
    // Enrich diagnostic context with CorrelationId and host, and set level based on exception presence/status.
    app.UseSerilogRequestLogging(opts =>
    {
        opts.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms (CorrelationId: {CorrelationId})";
        opts.GetLevel = (httpContext, elapsed, ex) =>
        {
            if (ex != null) return LogEventLevel.Error;
            var statusCode = httpContext.Response?.StatusCode;
            if (statusCode >= 500) return LogEventLevel.Error;
            if (statusCode >= 400) return LogEventLevel.Warning;
            return LogEventLevel.Information;
        };
        opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            var correlation = httpContext.Items.ContainsKey(CorrelationIdMiddleware.HeaderName)
                ? httpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString()
                : null;
            diagnosticContext.Set("CorrelationId", correlation ?? "none");
            diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        };
    });

    // Register exception logging middleware so it can produce sanitized responses and still be logged
    // by SerilogRequestLogging (Serilog will record exception-level if thrown).
    app.UseMiddleware<ExceptionLoggingMiddleware>();

    var versionDescProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.UseSwagger();

        app.UseSwaggerUI(opt =>
        {
            foreach(var desc in versionDescProvider.ApiVersionDescriptions)
            {
                opt.SwaggerEndpoint($"/swagger/{desc.GroupName}/swagger.json", $"Product API {desc.GroupName}");
            }    
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
