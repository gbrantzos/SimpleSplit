using Prometheus;

namespace SimpleSplit.Application
{
    public static class SimpleSplitMetrics
    {
        public static readonly Counter RequestsCounter = Metrics
            .CreateCounter("requests_counter", "Requests counter", new CounterConfiguration
            {
                LabelNames = new[] { "RequestType" }
            });

        public static readonly Histogram RequestsDuration = Metrics
            .CreateHistogram("requests_duration", "Requests duration", new HistogramConfiguration
            {
                LabelNames = new[] { "RequestType" },
                Buckets = Histogram.ExponentialBuckets(1, 2, 8)
            });
    }
}