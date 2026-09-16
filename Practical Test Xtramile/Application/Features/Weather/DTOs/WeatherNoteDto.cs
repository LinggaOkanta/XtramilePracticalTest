namespace Practical_Test_Xtramile.Application.Features.Weather.DTOs;

public record WeatherNoteDto(
    Guid Id,
    string CityName,
    string Note,
    double? TemperatureCelsius,
    string? SkyConditions,
    DateTime CreatedAtUtc
);

public record FavoriteCityDto(
    Guid Id,
    string CityName,
    string CountryCode,
    DateTime CreatedAtUtc
);
