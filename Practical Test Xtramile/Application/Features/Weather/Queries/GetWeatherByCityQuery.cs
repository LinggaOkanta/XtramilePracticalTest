using MediatR;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;

namespace Practical_Test_Xtramile.Application.Features.Weather.Queries;

public record GetWeatherByCityQuery(string CityName) : IRequest<WeatherResponseDto>;

public class GetWeatherByCityQueryHandler : IRequestHandler<GetWeatherByCityQuery, WeatherResponseDto>
{
    private readonly IWeatherService _weatherService;

    public GetWeatherByCityQueryHandler(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    public async Task<WeatherResponseDto> Handle(GetWeatherByCityQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CityName))
        {
            throw new ArgumentException("City name cannot be empty.", nameof(request.CityName));
        }

        return await _weatherService.GetCurrentWeatherAsync(request.CityName.Trim(), cancellationToken);
    }
}
