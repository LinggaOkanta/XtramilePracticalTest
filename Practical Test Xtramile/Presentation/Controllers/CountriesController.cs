using Microsoft.AspNetCore.Mvc;
using Practical_Test_Xtramile.Application.Features.Countries.DTOs;
using Practical_Test_Xtramile.Application.Features.Countries.Queries;

namespace Practical_Test_Xtramile.Presentation.Controllers;

[Route("api/countries")]
public class CountriesController : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CountryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> GetCountries()
    {
        var result = await Mediator.Send(new GetCountriesQuery());
        return Ok(result);
    }

    [HttpGet("{countryCode}/cities")]
    [ProducesResponseType(typeof(IReadOnlyList<CityDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<CityDto>>> GetCitiesByCountry(string countryCode)
    {
        var result = await Mediator.Send(new GetCitiesByCountryQuery(countryCode));
        return Ok(result);
    }
}
