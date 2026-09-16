using MediatR;
using Microsoft.EntityFrameworkCore;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Countries.DTOs;

namespace Practical_Test_Xtramile.Application.Features.Countries.Queries;

public record GetCountriesQuery : IRequest<IReadOnlyList<CountryDto>>;

public class GetCountriesQueryHandler : IRequestHandler<GetCountriesQuery, IReadOnlyList<CountryDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCountriesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<CountryDto>> Handle(GetCountriesQuery request, CancellationToken cancellationToken)
    {
        // Read Optimization: Explicitly using .AsNoTracking() as mandated by architectural rules
        var countries = await _context.Countries
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CountryDto(c.Id, c.Code, c.Name))
            .ToListAsync(cancellationToken);

        return countries;
    }
}
