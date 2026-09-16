using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Infrastructure.WeatherApi;

namespace Practical_Test_Xtramile.Tests.IntegrationTests;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Replace IWeatherService with MockWeatherService to ensure 0 external network calls
            var weatherServiceDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IWeatherService));
            if (weatherServiceDescriptor != null)
            {
                services.Remove(weatherServiceDescriptor);
            }
            services.AddSingleton<IWeatherService, MockWeatherService>();
        });
    }
}
