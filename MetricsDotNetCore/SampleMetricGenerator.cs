using App.Metrics;
using App.Metrics.Counter;
using App.Metrics.Meter;
using App.Metrics.Timer;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetricsDotNetCore
{
    public class SampleMetricGenerator : IHostedService
    {
        private readonly IMetrics _metrics;
        private Timer _timer;

        public SampleMetricGenerator(IMetrics metrics)
        {
            _metrics = metrics;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SampleMetricGenerator - StartAsync");

            _timer = new Timer(_ =>
            {
                try
                {
                    _metrics.MeasureTime("MyMetrics.Temporizador", () =>
                    {
                        _metrics.Count("MyMetrics.Chamadas", 1);
                        Thread.Sleep(new Random().Next(100, 1000));
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }, null, TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(new Random().Next(100, 1000)));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("SampleMetricGenerator - StopAsync");
            if(_timer != null)
            {
                _timer.Change(Timeout.Infinite, Timeout.Infinite);
            }
            return Task.CompletedTask;
        }
    }
}
