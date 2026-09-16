using Practical_Test_Xtramile.Application.Features.Weather.DTOs;

namespace Practical_Test_Xtramile.Application.Common.Interfaces;

public interface IWeatherService
{
    Task<WeatherResponseDto> GetCurrentWeatherAsync(string cityName, CancellationToken cancellationToken = default);
}
