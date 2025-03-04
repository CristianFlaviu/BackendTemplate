using Prometheus;

namespace BackendTemplate.Infrastructure.Metrics
{
    public static class MetricsConfig
    {
        // Define the histogram metric for HTTP request durations
        public static readonly Histogram CustomHttpRequestDuration = Prometheus.Metrics.CreateHistogram(
            "custom_http_request_duration_seconds",
            "Histogram of HTTP request durations in seconds",
            new HistogramConfiguration
            {
                Buckets = Histogram.LinearBuckets(0.01, 0.05, 20) // Customize the buckets as needed
            });
    }
}
