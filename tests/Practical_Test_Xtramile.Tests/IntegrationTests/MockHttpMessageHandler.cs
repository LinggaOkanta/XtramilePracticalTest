using System.Net;
using System.Text;

namespace Practical_Test_Xtramile.Tests.IntegrationTests;

public class MockHttpMessageHandler : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // Deterministic canned response matching OpenWeatherMap JSON structure
        const string cannedJson = """
        {
          "coord": { "lon": 144.9633, "lat": -37.814 },
          "weather": [{ "id": 800, "main": "Clear", "description": "clear sky", "icon": "01d" }],
          "main": { "temp": 68.0, "feels_like": 66.5, "temp_min": 65.0, "temp_max": 71.0, "pressure": 1013, "humidity": 55 },
          "visibility": 10000,
          "wind": { "speed": 12.5, "deg": 180 },
          "sys": { "country": "AU" },
          "dt": 1726454700,
          "name": "Melbourne"
        }
        """;

        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(cannedJson, Encoding.UTF8, "application/json")
        };

        return Task.FromResult(response);
    }
}
