using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Timer;
using System;

namespace MetricsDotNetCore
{
    public class AppMetricsImpl : IMetrics
    {
        private readonly IMetricsRoot _metricsRoot;
        public AppMetricsImpl(IMetricsRoot metricsRoot)
        {
            _metricsRoot = metricsRoot;
        }

        public void Count(string metricName, int increment)
        {
            var cacheHitsCounter = new CounterOptions
            {
                Context = GetMetricContext(metricName),
                Name = GetMetricName(metricName),
                MeasurementUnit = Unit.Calls
            };

            _metricsRoot.Measure.Counter.Increment(cacheHitsCounter, increment);
        }

        public void MeasureTime(string metricName, Action action)
        {
            var requestTimer = new TimerOptions
            {
                Context = GetMetricContext(metricName),
                Name = GetMetricName(metricName),
                MeasurementUnit = Unit.Calls,
                DurationUnit = TimeUnit.Seconds,
                RateUnit = TimeUnit.Seconds
            };
            _metricsRoot.Measure.Timer.Time(requestTimer, () => action());
        }

        private string GetMetricName(string metricName)
        {
            var tokens = metricName.Split('.');
            return tokens[tokens.Length - 1];
        }

        private string GetMetricContext(string metricName)
        {
            var tokens = metricName.Split('.');
            return tokens.Length == 2 ? tokens[0] : "appz";
        }
    }
}
