using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace SimpleProductAPI.Middleware
{
    public sealed class ExceptionLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionLoggingMiddleware> _logger;

        public ExceptionLoggingMiddleware(RequestDelegate next, ILogger<ExceptionLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log structured error with request context
                _logger.LogError(ex, "Unhandled exception for {Method} {Path}", context.Request?.Method, context.Request?.Path);

                // Ensure response is not already started
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("Response has already started when handling exception for {Path}", context.Request?.Path);
                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                // Don't leak exception details in production
                var env = context.RequestServices.GetService(typeof(IHostEnvironment)) as IHostEnvironment;
                var problem = new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "An unexpected error occurred."
                };

                if (env != null && env.IsDevelopment())
                {
                    // In development include the exception message and stack for easier debugging
                    problem.Detail = ex.ToString();
                }
                else
                {
                    problem.Detail = "An internal server error occurred. Please contact support with the correlation id.";
                }

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                var payload = JsonSerializer.Serialize(problem, options);
                await context.Response.WriteAsync(payload).ConfigureAwait(false);
            }
        }
    }
}