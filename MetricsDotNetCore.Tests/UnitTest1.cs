using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Linq;
using System.Net.Http;
using Xunit;
using MetricsDotNetCore;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;

namespace MetricsDotNetCore.Tests
{
    public class UnitTest1
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public UnitTest1()
        {
            // Arrange
            var webHost = 
            _server = new TestServer(new WebHostBuilder()
                .AddMetrics()
                .ConfigureServices((_, services) => services.AddHostedService<SampleMetricGenerator>())
                .UseKestrel()
                //.UseUrls("http://*:5000")
                .UseStartup<Startup>());
            _client = _server.CreateClient();
        }

        [Trait("Category", "Integration")]
        [Fact]
        public async void Test1()
        {
            // Act
            await Task.Delay(200);
            var response = await _client.GetAsync("/metrics");
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            // Assert
            Assert.Contains("mymetrics_chamadas gauge", responseString);
        }
    }
}
