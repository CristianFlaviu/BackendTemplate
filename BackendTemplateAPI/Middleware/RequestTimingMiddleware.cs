using BackendTemplate.Infrastructure.Metrics;
using System.Diagnostics;

namespace BackendTemplate.API.Middleware
{
    public class RequestTimingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestTimingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // Proceed with the next middleware in the pipeline
            await _next(context);

            stopwatch.Stop();

            // Record the duration of the request to Prometheus histogram
            MetricsConfig.CustomHttpRequestDuration.Observe(stopwatch.Elapsed.TotalSeconds);
        }
    }
}
