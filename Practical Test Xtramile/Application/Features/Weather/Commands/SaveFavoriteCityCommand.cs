using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;
using Practical_Test_Xtramile.Domain.Entities;

namespace Practical_Test_Xtramile.Application.Features.Weather.Commands;

public record SaveFavoriteCityCommand(string CityName, string CountryCode) : IRequest<FavoriteCityDto>;

public class SaveFavoriteCityValidator : AbstractValidator<SaveFavoriteCityCommand>
{
    public SaveFavoriteCityValidator()
    {
        RuleFor(v => v.CityName)
            .NotEmpty().WithMessage("City name is required.")
            .MaximumLength(100).WithMessage("City name must not exceed 100 characters.");

        RuleFor(v => v.CountryCode)
            .NotEmpty().WithMessage("Country code is required.")
            .Length(2, 3).WithMessage("Country code must be 2 or 3 characters.");
    }
}

public class SaveFavoriteCityCommandHandler : IRequestHandler<SaveFavoriteCityCommand, FavoriteCityDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SaveFavoriteCityCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<FavoriteCityDto> Handle(SaveFavoriteCityCommand request, CancellationToken cancellationToken)
    {
        string normalizedCity = request.CityName.Trim();
        string normalizedCountry = request.CountryCode.Trim().ToUpperInvariant();

        // Check if duplicate favorite exists
        var existing = await _context.FavoriteCities
            .FirstOrDefaultAsync(f => f.CityName.ToLower() == normalizedCity.ToLower() &&
                                      f.CountryCode.ToUpper() == normalizedCountry, cancellationToken);

        if (existing != null)
        {
            return new FavoriteCityDto(existing.Id, existing.CityName, existing.CountryCode, existing.CreatedAtUtc);
        }

        var entity = new FavoriteCity(
            id: Guid.NewGuid(),
            cityName: normalizedCity,
            countryCode: normalizedCountry,
            createdAtUtc: _dateTimeProvider.UtcNow
        );

        _context.FavoriteCities.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new FavoriteCityDto(entity.Id, entity.CityName, entity.CountryCode, entity.CreatedAtUtc);
    }
}
