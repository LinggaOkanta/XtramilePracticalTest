using Microsoft.AspNetCore.Mvc;
using Practical_Test_Xtramile.Application.Features.Weather.Commands;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;
using Practical_Test_Xtramile.Application.Features.Weather.Queries;

namespace Practical_Test_Xtramile.Presentation.Controllers;

[Route("api/[controller]")]
public class WeatherController : ApiControllerBase
{
    [HttpGet("{cityName}")]
    [ProducesResponseType(typeof(WeatherResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<WeatherResponseDto>> GetWeatherByCity(string cityName)
    {
        var result = await Mediator.Send(new GetWeatherByCityQuery(cityName));
        return Ok(result);
    }

    [HttpPost("notes")]
    [ProducesResponseType(typeof(WeatherNoteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<WeatherNoteDto>> CreateWeatherNote([FromBody] CreateWeatherNoteCommand command)
    {
        var result = await Mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    [HttpPost("favorites")]
    [HttpPost("/api/cities/favorites")]
    [ProducesResponseType(typeof(FavoriteCityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FavoriteCityDto>> SaveFavoriteCity([FromBody] SaveFavoriteCityCommand command)
    {
        var result = await Mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, result);
    }
}
