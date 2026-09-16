using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;
using Practical_Test_Xtramile.Domain.Services;
using Practical_Test_Xtramile.Domain.ValueObjects;

namespace Practical_Test_Xtramile.Infrastructure.WeatherApi;

public class MockWeatherService : IWeatherService
{
    private static readonly Dictionary<string, (string Country, string Code, double TempF, int Humidity, double WindMph, int WindDeg, string Sky)> CityProfiles =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["Melbourne"] = ("Australia", "AU", 68.0, 55, 12.5, 180, "Partly Cloudy"),
            ["Sydney"] = ("Australia", "AU", 72.5, 60, 10.0, 90, "Sunny"),
            ["Brisbane"] = ("Australia", "AU", 78.0, 65, 8.5, 120, "Clear"),
            ["Perth"] = ("Australia", "AU", 75.2, 50, 14.0, 220, "Clear"),
            ["New York"] = ("United States", "US", 62.0, 52, 9.0, 310, "Cloudy"),
            ["San Francisco"] = ("United States", "US", 58.0, 75, 15.5, 270, "Foggy"),
            ["Chicago"] = ("United States", "US", 55.4, 48, 18.0, 340, "Windy"),
            ["Seattle"] = ("United States", "US", 53.6, 80, 7.0, 200, "Rain"),
            ["Jakarta"] = ("Indonesia", "ID", 86.0, 80, 6.0, 45, "Thunderstorm"),
            ["Surabaya"] = ("Indonesia", "ID", 89.6, 70, 7.5, 60, "Sunny"),
            ["Bandung"] = ("Indonesia", "ID", 75.0, 78, 5.0, 110, "Scattered Clouds"),
            ["Bali"] = ("Indonesia", "ID", 84.2, 75, 8.0, 135, "Clear"),
            ["London"] = ("United Kingdom", "GB", 59.0, 68, 11.0, 240, "Drizzle"),
            ["Manchester"] = ("United Kingdom", "GB", 57.2, 72, 13.0, 260, "Overcast"),
            ["Edinburgh"] = ("United Kingdom", "GB", 53.6, 70, 16.0, 290, "Windy"),
            ["Tokyo"] = ("Japan", "JP", 66.2, 58, 8.0, 160, "Clear"),
            ["Osaka"] = ("Japan", "JP", 68.0, 62, 9.0, 170, "Partly Cloudy"),
            ["Kyoto"] = ("Japan", "JP", 64.4, 65, 6.5, 150, "Sunny")
        };

    public Task<WeatherResponseDto> GetCurrentWeatherAsync(string cityName, CancellationToken cancellationToken = default)
    {
        double tempF = 70.0;
        int humidity = 60;
        double windMph = 10.0;
        int windDeg = 180;
        string sky = "Clear";
        string country = "Global";
        string countryCode = "GL";

        if (CityProfiles.TryGetValue(cityName, out var profile))
        {
            country = profile.Country;
            countryCode = profile.Code;
            tempF = profile.TempF;
            humidity = profile.Humidity;
            windMph = profile.WindMph;
            windDeg = profile.WindDeg;
            sky = profile.Sky;
        }

        // Domain calculation rules: Pure temperature conversion
        double tempC = TemperatureConverter.FahrenheitToCelsius(tempF);
        double dewPointC = TemperatureConverter.CalculateDewPointCelsius(tempC, humidity);
        double dewPointF = TemperatureConverter.CelsiusToFahrenheit(dewPointC);
        var wind = WindInfo.Create(windMph, windDeg);

        var dto = new WeatherResponseDto
        {
            Location = new LocationDto(cityName, country, countryCode),
            TimeUtc = DateTime.UtcNow,
            Wind = new WindDto(wind.SpeedMph, wind.DirectionDegrees, wind.DirectionCardinal),
            VisibilityMeters = 10000,
            PressureHpa = 1013.25,
            SkyConditions = sky,
            Temperature = new TemperatureDto(Math.Round(tempF, 2), tempC),
            DewPoint = new TemperatureDto(dewPointF, dewPointC),
            RelativeHumidityPercent = humidity
        };

        return Task.FromResult(dto);
    }
}
