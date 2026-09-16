using FluentAssertions;
using Practical_Test_Xtramile.Domain.Enums;
using Xunit;

namespace Practical_Test_Xtramile.Tests.UnitTests.Domain;

public class SkyConditionEnumTests
{
    [Theory]
    [InlineData("Thunderstorm", SkyCondition.Thunderstorm)]
    [InlineData("Light Rain", SkyCondition.Rain)]
    [InlineData("Heavy Drizzle", SkyCondition.Drizzle)]
    [InlineData("Snow Flurries", SkyCondition.Snow)]
    [InlineData("Dense Fog", SkyCondition.Fog)]
    [InlineData("Morning Mist", SkyCondition.Mist)]
    [InlineData("Windy conditions", SkyCondition.Windy)]
    [InlineData("Sunny Skies", SkyCondition.Sunny)]
    [InlineData("Partly Cloudy", SkyCondition.PartlyCloudy)]
    [InlineData("Overcast", SkyCondition.Overcast)]
    [InlineData("Scattered Clouds", SkyCondition.Cloudy)]
    [InlineData("Clear Sky", SkyCondition.Clear)]
    [InlineData("Unknown Condition 123", SkyCondition.Unknown)]
    [InlineData("", SkyCondition.Unknown)]
    [InlineData(null, SkyCondition.Unknown)]
    public void ParseSkyCondition_ShouldMapToAccurateEnum(string? input, SkyCondition expected)
    {
        // Act
        var result = SkyConditionExtensions.ParseSkyCondition(input);

        // Assert
        result.Should().Be(expected);
    }
}
