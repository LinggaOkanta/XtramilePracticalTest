using Practical_Test_Xtramile.Domain.Enums;
using Practical_Test_Xtramile.Domain.Services;

namespace Practical_Test_Xtramile.Domain.ValueObjects;

public record Temperature
{
    public double Fahrenheit { get; init; }
    public double Celsius { get; init; }

    public static Temperature FromFahrenheit(double fahrenheit)
    {
        return new Temperature
        {
            Fahrenheit = Math.Round(fahrenheit, 2, MidpointRounding.AwayFromZero),
            Celsius = TemperatureConverter.FahrenheitToCelsius(fahrenheit)
        };
    }

    public static Temperature FromCelsius(double celsius)
    {
        return new Temperature
        {
            Celsius = Math.Round(celsius, 2, MidpointRounding.AwayFromZero),
            Fahrenheit = TemperatureConverter.CelsiusToFahrenheit(celsius)
        };
    }

    public static Temperature FromUnit(double value, TemperatureUnit unit) => unit switch
    {
        TemperatureUnit.Fahrenheit => FromFahrenheit(value),
        TemperatureUnit.Celsius => FromCelsius(value),
        _ => throw new ArgumentOutOfRangeException(nameof(unit), unit, null)
    };
}

