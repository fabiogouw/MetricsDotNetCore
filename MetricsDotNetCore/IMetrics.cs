using System;

namespace MetricsDotNetCore
{
    public interface IMetrics
    {
        void Count(string metricName, int increment);
        void MeasureTime(string metricName, Action action);
    }
}
