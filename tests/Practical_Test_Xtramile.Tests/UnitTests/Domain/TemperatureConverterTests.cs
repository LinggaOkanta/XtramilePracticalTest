using FluentAssertions;
using Practical_Test_Xtramile.Domain.Services;
using Practical_Test_Xtramile.Domain.ValueObjects;
using Xunit;

namespace Practical_Test_Xtramile.Tests.UnitTests.Domain;

public class TemperatureConverterTests
{
    [Theory]
    [InlineData(32.0, 0.0)]         // Freezing Point
    [InlineData(212.0, 100.0)]      // Boiling Point
    [InlineData(98.6, 37.0)]        // Body Temperature
    [InlineData(-40.0, -40.0)]      // Crossover Point (-40F == -40C)
    [InlineData(-459.67, -273.15)]  // Absolute Zero
    [InlineData(0.0, -17.78)]       // Zero Fahrenheit
    [InlineData(68.0, 20.0)]        // Room Temperature
    public void FahrenheitToCelsius_ShouldConvertAccurately_AndRoundToTwoDecimals(double fahrenheit, double expectedCelsius)
    {
        // Act
        double actual = TemperatureConverter.FahrenheitToCelsius(fahrenheit);

        // Assert
        actual.Should().Be(expectedCelsius);
    }

    [Theory]
    [InlineData(0.0, 32.0)]
    [InlineData(100.0, 212.0)]
    [InlineData(37.0, 98.6)]
    [InlineData(-40.0, -40.0)]
    [InlineData(-273.15, -459.67)]
    [InlineData(20.0, 68.0)]
    public void CelsiusToFahrenheit_ShouldConvertAccurately_AndRoundToTwoDecimals(double celsius, double expectedFahrenheit)
    {
        // Act
        double actual = TemperatureConverter.CelsiusToFahrenheit(celsius);

        // Assert
        actual.Should().Be(expectedFahrenheit);
    }

    [Fact]
    public void Temperature_FromFahrenheit_ShouldPopulateBothUnitsCorrectly()
    {
        // Act
        var temp = Temperature.FromFahrenheit(68.0);

        // Assert
        temp.Fahrenheit.Should().Be(68.0);
        temp.Celsius.Should().Be(20.0);
    }

    [Fact]
    public void Temperature_FromCelsius_ShouldPopulateBothUnitsCorrectly()
    {
        // Act
        var temp = Temperature.FromCelsius(20.0);

        // Assert
        temp.Celsius.Should().Be(20.0);
        temp.Fahrenheit.Should().Be(68.0);
    }

    [Theory]
    [InlineData(20.0, 50.0, 9.27)]
    [InlineData(25.0, 60.0, 16.71)]
    public void CalculateDewPointCelsius_ShouldCalculateMagnusFormulaAccurately(double tempC, double humidity, double expectedDewPoint)
    {
        // Act
        double dewPoint = TemperatureConverter.CalculateDewPointCelsius(tempC, humidity);

        // Assert
        dewPoint.Should().BeApproximately(expectedDewPoint, 0.1);
    }

    [Theory]
    [InlineData(68.0, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Fahrenheit, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Celsius, 20.0)]
    [InlineData(20.0, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Celsius, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Fahrenheit, 68.0)]
    [InlineData(25.5, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Celsius, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Celsius, 25.5)]
    public void Convert_WithTemperatureUnitEnum_ShouldConvertAccurately(
        double value,
        Practical_Test_Xtramile.Domain.Enums.TemperatureUnit fromUnit,
        Practical_Test_Xtramile.Domain.Enums.TemperatureUnit toUnit,
        double expected)
    {
        // Act
        double result = TemperatureConverter.Convert(value, fromUnit, toUnit);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Temperature_FromUnit_WithTemperatureUnitEnum_ShouldCreateValidTemperatureObject()
    {
        // Act
        var tempF = Temperature.FromUnit(68.0, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Fahrenheit);
        var tempC = Temperature.FromUnit(20.0, Practical_Test_Xtramile.Domain.Enums.TemperatureUnit.Celsius);

        // Assert
        tempF.Celsius.Should().Be(20.0);
        tempF.Fahrenheit.Should().Be(68.0);

        tempC.Celsius.Should().Be(20.0);
        tempC.Fahrenheit.Should().Be(68.0);
    }
}

