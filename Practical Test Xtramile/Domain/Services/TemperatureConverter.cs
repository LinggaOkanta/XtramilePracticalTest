using Practical_Test_Xtramile.Domain.Enums;

namespace Practical_Test_Xtramile.Domain.Services;

public static class TemperatureConverter
{
    /// <summary>
    /// Converts Fahrenheit to Celsius rounded to 2 decimal places using AwayFromZero.
    /// Formula: (Fahrenheit - 32) * 5 / 9
    /// </summary>
    public static double FahrenheitToCelsius(double fahrenheit)
    {
        double celsius = (fahrenheit - 32.0) * 5.0 / 9.0;
        return Math.Round(celsius, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Converts Celsius to Fahrenheit rounded to 2 decimal places using AwayFromZero.
    /// Formula: (Celsius * 9 / 5) + 32
    /// </summary>
    public static double CelsiusToFahrenheit(double celsius)
    {
        double fahrenheit = (celsius * 9.0 / 5.0) + 32.0;
        return Math.Round(fahrenheit, 2, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Converts a temperature value from one unit to another using the strongly-typed TemperatureUnit enum.
    /// </summary>
    public static double Convert(double value, TemperatureUnit fromUnit, TemperatureUnit toUnit)
    {
        if (fromUnit == toUnit)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        return (fromUnit, toUnit) switch
        {
            (TemperatureUnit.Fahrenheit, TemperatureUnit.Celsius) => FahrenheitToCelsius(value),
            (TemperatureUnit.Celsius, TemperatureUnit.Fahrenheit) => CelsiusToFahrenheit(value),
            _ => throw new ArgumentOutOfRangeException($"Unsupported conversion from {fromUnit} to {toUnit}")
        };
    }

    /// <summary>
    /// Approximates Dew Point using the Magnus-Tetens formula or NOAA approximation:
    /// Td ≈ T - ((100 - RH) / 5)
    /// </summary>
    public static double CalculateDewPointCelsius(double temperatureCelsius, double relativeHumidityPercent)
    {
        if (relativeHumidityPercent <= 0) relativeHumidityPercent = 1;
        if (relativeHumidityPercent > 100) relativeHumidityPercent = 100;

        // Magnus-Tetens formula constants
        const double a = 17.27;
        const double b = 237.7;
        double alpha = ((a * temperatureCelsius) / (b + temperatureCelsius)) + Math.Log(relativeHumidityPercent / 100.0);
        double dewPoint = (b * alpha) / (a - alpha);
        return Math.Round(dewPoint, 2, MidpointRounding.AwayFromZero);
    }
}

