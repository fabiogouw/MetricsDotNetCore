using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using App.Metrics.Meter;
using App.Metrics.Timer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MetricsDotNetCore
{
    public class Program
    {
        private static Timer _timer;
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var metrics = AppMetrics.CreateDefaultBuilder()
               .OutputMetrics.AsPrometheusPlainText()
               .Build();

            var requestTimer = new TimerOptions
            {
                Context = "My Metrics",
                Name = "Temporizador",
                MeasurementUnit = Unit.Events,
                DurationUnit = TimeUnit.Seconds,
                RateUnit = TimeUnit.Seconds
            };

            var cacheHitsMeter = new MeterOptions
            {
                Context = "My Metrics",
                Name = "Chamadas",
                MeasurementUnit = Unit.Calls,
                RateUnit = TimeUnit.Seconds,
            };

            _timer = new Timer(_ =>
            {
                try
                {
                    metrics.Measure.Timer.Time(requestTimer, () =>
                    {
                        metrics.Measure.Meter.Mark(cacheHitsMeter);
                        Thread.Sleep(new Random().Next(100, 1000));
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(new Random().Next(100, 1000)));

            return Host.CreateDefaultBuilder(args)
                .ConfigureMetrics(metrics)
                .UseMetrics(
                            options =>
                            {
                                options.EndpointOptions = endpointsOptions =>
                                {
                                    endpointsOptions.EnvironmentInfoEndpointEnabled = false;
                                    endpointsOptions.MetricsEndpointEnabled = true;
                                    endpointsOptions.MetricsTextEndpointEnabled = true;
                                    endpointsOptions.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                                    endpointsOptions.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                                };
                            })
                .ConfigureServices(services => services.AddAppMetricsCollectors())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseUrls("http://*:5000");
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
}
