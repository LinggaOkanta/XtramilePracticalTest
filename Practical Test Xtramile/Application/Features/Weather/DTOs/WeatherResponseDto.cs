namespace Practical_Test_Xtramile.Application.Features.Weather.DTOs;

public record LocationDto(string City, string Country, string CountryCode);

public record WindDto(double SpeedMph, int DirectionDegrees, string DirectionCardinal);

public record TemperatureDto(double Fahrenheit, double Celsius);

public record WeatherResponseDto
{
    public LocationDto Location { get; init; } = default!;
    public DateTime TimeUtc { get; init; }
    public WindDto Wind { get; init; } = default!;
    public int VisibilityMeters { get; init; }
    public double PressureHpa { get; init; }
    public string SkyConditions { get; init; } = string.Empty;
    public TemperatureDto Temperature { get; init; } = default!;
    public TemperatureDto DewPoint { get; init; } = default!;
    public int RelativeHumidityPercent { get; init; }
}
