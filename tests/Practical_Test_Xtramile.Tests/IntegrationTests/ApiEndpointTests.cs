using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Practical_Test_Xtramile.Application.Features.Countries.DTOs;
using Practical_Test_Xtramile.Application.Features.Weather.Commands;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;
using Xunit;

namespace Practical_Test_Xtramile.Tests.IntegrationTests;

public class ApiEndpointTests : IClassFixture<TestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ApiEndpointTests(TestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCountries_ReturnsSuccessStatusCode_AndCountriesList()
    {
        // Act
        var response = await _client.GetAsync("/api/countries");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var countries = await response.Content.ReadFromJsonAsync<List<CountryDto>>();
        countries.Should().NotBeNull();
        countries!.Should().NotBeEmpty();
        countries.Should().Contain(c => c.Code == "AU");
        countries.Should().Contain(c => c.Code == "ID");
        countries.Should().Contain(c => c.Code == "US");
    }

    [Fact]
    public async Task GetCitiesByCountry_ValidCountryCode_ReturnsCitiesList()
    {
        // Act
        var response = await _client.GetAsync("/api/countries/AU/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var cities = await response.Content.ReadFromJsonAsync<List<CityDto>>();
        cities.Should().NotBeNull();
        cities!.Should().NotBeEmpty();
        cities.Should().Contain(c => c.Name == "Melbourne");
        cities.Should().Contain(c => c.Name == "Sydney");
    }

    [Fact]
    public async Task GetCitiesByCountry_InvalidCountryCode_ReturnsNotFound()
    {
        // Act
        var response = await _client.GetAsync("/api/countries/XYZ_INVALID/cities");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetWeatherByCity_ValidCity_ReturnsCompleteWeatherTelemetry()
    {
        // Act
        var response = await _client.GetAsync("/api/weather/Melbourne");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var weather = await response.Content.ReadFromJsonAsync<WeatherResponseDto>();
        weather.Should().NotBeNull();
        weather!.Location.City.Should().Be("Melbourne");
        weather.Temperature.Celsius.Should().Be(20.0);
        weather.Temperature.Fahrenheit.Should().Be(68.0);
        weather.Wind.Should().NotBeNull();
        weather.DewPoint.Should().NotBeNull();
        weather.RelativeHumidityPercent.Should().BeGreaterThan(0);
        weather.SkyConditions.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task CreateWeatherNote_ValidPayload_Returns201CreatedWithResource()
    {
        // Arrange
        var command = new CreateWeatherNoteCommand(
            CityName: "Melbourne",
            Note: "Optimal weather for integration tests.",
            TemperatureCelsius: 20.0,
            SkyConditions: "Clear"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/weather/notes", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<WeatherNoteDto>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.CityName.Should().Be("Melbourne");
        result.Note.Should().Be("Optimal weather for integration tests.");
    }

    [Fact]
    public async Task CreateWeatherNote_EmptyNote_Returns400BadRequestValidationProblemDetails()
    {
        // Arrange - Note is empty, which violates FluentValidation rule
        var invalidCommand = new CreateWeatherNoteCommand(
            CityName: "Sydney",
            Note: "",
            TemperatureCelsius: 22.0,
            SkyConditions: "Sunny"
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/weather/notes", invalidCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        problem.Should().NotBeNull();
        problem!.Errors.Should().ContainKey("Note");
    }

    [Fact]
    public async Task SaveFavoriteCity_ValidPayload_Returns201CreatedWithResource()
    {
        // Arrange
        var command = new SaveFavoriteCityCommand("Sydney", "AU");

        // Act
        var response = await _client.PostAsJsonAsync("/api/cities/favorites", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<FavoriteCityDto>();
        result.Should().NotBeNull();
        result!.Id.Should().NotBeEmpty();
        result.CityName.Should().Be("Sydney");
        result.CountryCode.Should().Be("AU");
    }
}
