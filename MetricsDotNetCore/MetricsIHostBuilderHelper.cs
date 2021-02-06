using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace MetricsDotNetCore
{
    public static class MetricsIHostBuilderHelper
    {
        public static IHostBuilder AddMetrics(this IHostBuilder hostBuilder)
        {
            var metrics = AppMetrics.CreateDefaultBuilder()
               .OutputMetrics.AsPrometheusPlainText()
               .Build();
            hostBuilder = hostBuilder.ConfigureMetrics(metrics)
                .UseMetrics(options =>
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
                .ConfigureServices(services =>
                {
                    services.AddAppMetricsCollectors();
                    services.AddSingleton<IMetricsRoot>(metrics);
                    services.AddSingleton<IMetrics, AppMetricsImpl>();
                });
            return hostBuilder;
        }

        public static IWebHostBuilder AddMetrics(this IWebHostBuilder hostBuilder)
        {
            var metrics = AppMetrics.CreateDefaultBuilder()
               .OutputMetrics.AsPrometheusPlainText()
               .Build();
            hostBuilder = hostBuilder.ConfigureMetrics(metrics)
                .UseMetrics(options =>
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
                .ConfigureServices(services =>
                {
                    services.AddAppMetricsCollectors();
                    services.AddSingleton<IMetricsRoot>(metrics);
                    services.AddSingleton<IMetrics, AppMetricsImpl>();
                });
            return hostBuilder;
        }
    }
}
