using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Application.Features.Weather.Commands;
using Practical_Test_Xtramile.Infrastructure.Persistence;
using Xunit;

namespace Practical_Test_Xtramile.Tests.UnitTests.Application.Commands;

public class CreateWeatherNoteCommandHandlerTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IDateTimeProvider> _dateTimeProviderMock;
    private readonly CreateWeatherNoteCommandHandler _handler;
    private readonly DateTime _fixedUtcTime = new(2026, 9, 16, 3, 0, 0, DateTimeKind.Utc);

    public CreateWeatherNoteCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: $"WeatherNoteDb_{Guid.NewGuid()}")
            .Options;

        _context = new ApplicationDbContext(options);

        _dateTimeProviderMock = new Mock<IDateTimeProvider>();
        _dateTimeProviderMock.Setup(d => d.UtcNow).Returns(_fixedUtcTime);

        _handler = new CreateWeatherNoteCommandHandler(_context, _dateTimeProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_ShouldPersistEntityAndReturnDtoWithGuid()
    {
        // Arrange
        var command = new CreateWeatherNoteCommand(
            CityName: "Melbourne",
            Note: "High wind velocity observed near Southbank.",
            TemperatureCelsius: 21.5,
            SkyConditions: "Partly Cloudy"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert - DTO verification
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.CityName.Should().Be("Melbourne");
        result.Note.Should().Be("High wind velocity observed near Southbank.");
        result.TemperatureCelsius.Should().Be(21.5);
        result.SkyConditions.Should().Be("Partly Cloudy");
        result.CreatedAtUtc.Should().Be(_fixedUtcTime);

        // Assert - EF Core In-Memory State Verification
        var persisted = await _context.WeatherNotes.FirstOrDefaultAsync(w => w.Id == result.Id);
        persisted.Should().NotBeNull();
        persisted!.CityName.Should().Be("Melbourne");
        persisted.Note.Should().Be("High wind velocity observed near Southbank.");
        persisted.CreatedAtUtc.Should().Be(_fixedUtcTime);
    }

    [Fact]
    public void Validator_EmptyNoteOrCity_ShouldHaveValidationErrors()
    {
        // Arrange
        var validator = new CreateWeatherNoteValidator();
        var invalidCommand = new CreateWeatherNoteCommand("", "");

        // Act
        var result = validator.Validate(invalidCommand);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateWeatherNoteCommand.CityName));
        result.Errors.Should().Contain(e => e.PropertyName == nameof(CreateWeatherNoteCommand.Note));
    }

    [Fact]
    public void Validator_ValidCommand_ShouldPassValidation()
    {
        // Arrange
        var validator = new CreateWeatherNoteValidator();
        var validCommand = new CreateWeatherNoteCommand("Sydney", "Clear skies all afternoon");

        // Act
        var result = validator.Validate(validCommand);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}
