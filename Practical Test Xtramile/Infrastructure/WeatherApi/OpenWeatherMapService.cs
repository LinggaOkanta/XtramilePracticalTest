using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Practical_Test_Xtramile.Application.Common.Exceptions;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;
using Practical_Test_Xtramile.Domain.Services;
using Practical_Test_Xtramile.Domain.ValueObjects;
using Practical_Test_Xtramile.Infrastructure.WeatherApi.Models;

namespace Practical_Test_Xtramile.Infrastructure.WeatherApi;

public class OpenWeatherMapService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly OpenWeatherMapSettings _settings;
    private readonly MockWeatherService _mockFallback;
    private readonly ILogger<OpenWeatherMapService> _logger;

    public OpenWeatherMapService(
        HttpClient httpClient,
        IOptions<OpenWeatherMapSettings> options,
        ILogger<OpenWeatherMapService> logger)
    {
        _httpClient = httpClient;
        _settings = options.Value;
        _mockFallback = new MockWeatherService();
        _logger = logger;
    }

    public async Task<WeatherResponseDto> GetCurrentWeatherAsync(string cityName, CancellationToken cancellationToken = default)
    {
        // If configured to use mock or if API key is not configured, seamlessly use the offline mock
        if (_settings.UseMock ||
            string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            _settings.ApiKey.Equals("YOUR_OPENWEATHERMAP_API_KEY", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("Using MockWeatherService for city '{City}' (Offline/Mock Mode).", cityName);
            return await _mockFallback.GetCurrentWeatherAsync(cityName, cancellationToken);
        }

        string requestUrl = $"{_settings.BaseUrl.TrimEnd('/')}/weather?q={Uri.EscapeDataString(cityName)}&units=imperial&appid={_settings.ApiKey}";

        try
        {
            var response = await _httpClient.GetAsync(requestUrl, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new NotFoundException("Weather data for city", cityName);
            }

            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadFromJsonAsync<OpenWeatherRawResponse>(cancellationToken: cancellationToken);

            if (raw == null || raw.Main == null)
            {
                throw new InvalidOperationException("Invalid response received from OpenWeatherMap API.");
            }

            // External OpenWeatherMap imperial telemetry: Temperature is in Fahrenheit
            double tempF = raw.Main.Temp;
            int humidity = raw.Main.Humidity;

            // Pure Domain Logic: Temperature & Dew Point conversions
            double tempC = TemperatureConverter.FahrenheitToCelsius(tempF);
            double dewPointC = TemperatureConverter.CalculateDewPointCelsius(tempC, humidity);
            double dewPointF = TemperatureConverter.CelsiusToFahrenheit(dewPointC);

            double windSpeed = raw.Wind?.Speed ?? 0.0;
            int windDeg = raw.Wind?.Deg ?? 0;
            var wind = WindInfo.Create(windSpeed, windDeg);

            string skyCondition = raw.Weather?.FirstOrDefault()?.Main ?? "Clear";
            string countryCode = raw.Sys?.Country ?? string.Empty;

            return new WeatherResponseDto
            {
                Location = new LocationDto(raw.Name ?? cityName, countryCode, countryCode),
                TimeUtc = raw.Dt > 0 ? DateTimeOffset.FromUnixTimeSeconds(raw.Dt).UtcDateTime : DateTime.UtcNow,
                Wind = new WindDto(wind.SpeedMph, wind.DirectionDegrees, wind.DirectionCardinal),
                VisibilityMeters = raw.Visibility,
                PressureHpa = raw.Main.Pressure,
                SkyConditions = skyCondition,
                Temperature = new TemperatureDto(Math.Round(tempF, 2), tempC),
                DewPoint = new TemperatureDto(dewPointF, dewPointC),
                RelativeHumidityPercent = humidity
            };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "HTTP request to OpenWeatherMap failed for city '{City}'. Falling back to Mock service.", cityName);
            return await _mockFallback.GetCurrentWeatherAsync(cityName, cancellationToken);
        }
    }
}
