using Serilog.Context;

namespace SimpleProductAPI.Middleware
{
    public sealed class CorrelationIdMiddleware
    {
        public const string HeaderName = "X-Correlation-ID";

        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Try to reuse incoming correlation id, otherwise generate a new one.
            var incomingId = context.Request.Headers.ContainsKey(HeaderName)
                ? context.Request.Headers[HeaderName].FirstOrDefault()
                : null;

            var correlationId = !string.IsNullOrWhiteSpace(incomingId)
                ? incomingId!
                : Guid.NewGuid().ToString("D");

            // Ensure response includes correlation id for client troubleshooting
            context.Response.OnStarting(() =>
            {
                if (!context.Response.Headers.ContainsKey(HeaderName))
                {
                    context.Response.Headers.Append(HeaderName, correlationId);
                }
                return Task.CompletedTask;
            });

            // Push into Serilog LogContext so ILogger<T> logs include the property when .Enrich.FromLogContext() is enabled
            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                // Optionally expose to downstream code via HttpContext.Items
                context.Items[HeaderName] = correlationId;
                await _next(context).ConfigureAwait(false);
            }
        }
    }
}