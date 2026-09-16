namespace Practical_Test_Xtramile.Domain.Enums;

/// <summary>
/// Atmospheric and sky condition categories.
/// </summary>
public enum SkyCondition
{
    Clear,
    Sunny,
    PartlyCloudy,
    Cloudy,
    Overcast,
    Rain,
    Drizzle,
    Thunderstorm,
    Snow,
    Fog,
    Mist,
    Windy,
    Unknown
}

/// <summary>
/// Helper extensions for parsing and working with SkyCondition enum.
/// </summary>
public static class SkyConditionExtensions
{
    public static SkyCondition ParseSkyCondition(string? condition)
    {
        if (string.IsNullOrWhiteSpace(condition))
        {
            return SkyCondition.Unknown;
        }

        string normalized = condition.Trim().ToLowerInvariant();

        if (normalized.Contains("thunder")) return SkyCondition.Thunderstorm;
        if (normalized.Contains("drizzle")) return SkyCondition.Drizzle;
        if (normalized.Contains("rain")) return SkyCondition.Rain;
        if (normalized.Contains("snow")) return SkyCondition.Snow;
        if (normalized.Contains("fog")) return SkyCondition.Fog;
        if (normalized.Contains("mist")) return SkyCondition.Mist;
        if (normalized.Contains("wind")) return SkyCondition.Windy;
        if (normalized.Contains("sun")) return SkyCondition.Sunny;
        if (normalized.Contains("partly")) return SkyCondition.PartlyCloudy;
        if (normalized.Contains("overcast")) return SkyCondition.Overcast;
        if (normalized.Contains("cloud")) return SkyCondition.Cloudy;
        if (normalized.Contains("clear")) return SkyCondition.Clear;

        return SkyCondition.Unknown;
    }
}
