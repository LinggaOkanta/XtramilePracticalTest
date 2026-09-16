using FluentAssertions;
using Practical_Test_Xtramile.Domain.Enums;
using Practical_Test_Xtramile.Domain.ValueObjects;
using Xunit;

namespace Practical_Test_Xtramile.Tests.UnitTests.Domain;

public class WindInfoEnumTests
{
    [Theory]
    [InlineData(0, CardinalDirection.N, "N")]
    [InlineData(360, CardinalDirection.N, "N")]
    [InlineData(45, CardinalDirection.NE, "NE")]
    [InlineData(90, CardinalDirection.E, "E")]
    [InlineData(135, CardinalDirection.SE, "SE")]
    [InlineData(180, CardinalDirection.S, "S")]
    [InlineData(225, CardinalDirection.SW, "SW")]
    [InlineData(270, CardinalDirection.W, "W")]
    [InlineData(315, CardinalDirection.NW, "NW")]
    public void DegreesToCardinal_ShouldMapToAccurateCardinalDirectionEnum(int degrees, CardinalDirection expectedEnum, string expectedString)
    {
        // Act
        var cardinal = WindInfo.DegreesToCardinal(degrees);
        var windInfo = WindInfo.Create(10.5, degrees);

        // Assert
        cardinal.Should().Be(expectedEnum);
        windInfo.CardinalDirection.Should().Be(expectedEnum);
        windInfo.DirectionCardinal.Should().Be(expectedString);
    }
}
