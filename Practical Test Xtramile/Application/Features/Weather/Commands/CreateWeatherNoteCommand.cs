using FluentValidation;
using MediatR;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Weather.DTOs;
using Practical_Test_Xtramile.Domain.Entities;

namespace Practical_Test_Xtramile.Application.Features.Weather.Commands;

public record CreateWeatherNoteCommand(
    string CityName,
    string Note,
    double? TemperatureCelsius = null,
    string? SkyConditions = null
) : IRequest<WeatherNoteDto>;

public class CreateWeatherNoteValidator : AbstractValidator<CreateWeatherNoteCommand>
{
    public CreateWeatherNoteValidator()
    {
        RuleFor(v => v.CityName)
            .NotEmpty().WithMessage("City name is required.")
            .MaximumLength(100).WithMessage("City name must not exceed 100 characters.");

        RuleFor(v => v.Note)
            .NotEmpty().WithMessage("Note cannot be empty.")
            .MaximumLength(500).WithMessage("Note must not exceed 500 characters.");
    }
}

public class CreateWeatherNoteCommandHandler : IRequestHandler<CreateWeatherNoteCommand, WeatherNoteDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateWeatherNoteCommandHandler(IApplicationDbContext context, IDateTimeProvider dateTimeProvider)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<WeatherNoteDto> Handle(CreateWeatherNoteCommand request, CancellationToken cancellationToken)
    {
        var entity = new WeatherNote(
            id: Guid.NewGuid(),
            cityName: request.CityName,
            note: request.Note,
            temperatureCelsius: request.TemperatureCelsius,
            skyConditions: request.SkyConditions,
            createdAtUtc: _dateTimeProvider.UtcNow
        );

        _context.WeatherNotes.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);

        return new WeatherNoteDto(
            entity.Id,
            entity.CityName,
            entity.Note,
            entity.TemperatureCelsius,
            entity.SkyConditions,
            entity.CreatedAtUtc
        );
    }
}
