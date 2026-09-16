using MediatR;
using Microsoft.EntityFrameworkCore;
using Practical_Test_Xtramile.Application.Common.Exceptions;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Countries.DTOs;

namespace Practical_Test_Xtramile.Application.Features.Countries.Queries;

public record GetCitiesByCountryQuery(string CountryCode) : IRequest<IReadOnlyList<CityDto>>;

public class GetCitiesByCountryQueryHandler : IRequestHandler<GetCitiesByCountryQuery, IReadOnlyList<CityDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCitiesByCountryQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CityDto>> Handle(GetCitiesByCountryQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CountryCode))
        {
            throw new ValidationException(new[]
            {
                new FluentValidation.Results.ValidationFailure(nameof(request.CountryCode), "Country code is required.")
            });
        }

        string normalizedCode = request.CountryCode.Trim().ToUpperInvariant();

        // Check if country exists
        var country = await _context.Countries
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Code.ToUpper() == normalizedCode, cancellationToken);

        if (country == null)
        {
            throw new NotFoundException("Country", request.CountryCode);
        }

        // Read Optimization: Explicitly using .AsNoTracking() as mandated by architectural rules
        var cities = await _context.Cities
            .AsNoTracking()
            .Where(c => c.CountryId == country.Id)
            .OrderBy(c => c.Name)
            .Select(c => new CityDto(c.Id, c.Name, country.Code))
            .ToListAsync(cancellationToken);

        return cities;
    }
}
