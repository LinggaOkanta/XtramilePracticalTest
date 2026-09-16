using FluentAssertions;
using Moq;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;
using Practical_Test_Xtramile.Application.Features.Weather.Queries;
using Xunit;

namespace Practical_Test_Xtramile.Tests.UnitTests.Application.Queries;

public class GetWeatherByCityQueryHandlerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly GetWeatherByCityQueryHandler _handler;

    public GetWeatherByCityQueryHandlerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _handler = new GetWeatherByCityQueryHandler(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCityName_ShouldReturnWeatherDtoFromService()
    {
        // Arrange
        var expectedResponse = new WeatherResponseDto
        {
            Location = new LocationDto("Melbourne", "Australia", "AU"),
            TimeUtc = DateTime.UtcNow,
            Wind = new WindDto(12.5, 180, "S"),
            VisibilityMeters = 10000,
            PressureHpa = 1013.25,
            SkyConditions = "Clear",
            Temperature = new TemperatureDto(68.0, 20.0),
            DewPoint = new TemperatureDto(50.0, 10.0),
            RelativeHumidityPercent = 55
        };

        _weatherServiceMock
            .Setup(s => s.GetCurrentWeatherAsync("Melbourne", It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        var query = new GetWeatherByCityQuery("Melbourne");

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Location.City.Should().Be("Melbourne");
        result.Temperature.Celsius.Should().Be(20.0);
        result.Temperature.Fahrenheit.Should().Be(68.0);
        result.RelativeHumidityPercent.Should().Be(55);

        _weatherServiceMock.Verify(s => s.GetCurrentWeatherAsync("Melbourne", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_InvalidCityName_ShouldThrowArgumentException(string? invalidCity)
    {
        // Arrange
        var query = new GetWeatherByCityQuery(invalidCity!);

        // Act
        Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }
}
