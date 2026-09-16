namespace Practical_Test_Xtramile.Domain.ValueObjects;

public record WindInfo
{
    public double SpeedMph { get; init; }
    public int DirectionDegrees { get; init; }
    public string DirectionCardinal { get; init; } = string.Empty;

    public static WindInfo Create(double speedMph, int directionDegrees)
    {
        return new WindInfo
        {
            SpeedMph = Math.Round(speedMph, 2, MidpointRounding.AwayFromZero),
            DirectionDegrees = directionDegrees,
            DirectionCardinal = DegreesToCardinal(directionDegrees)
        };
    }

    public static string DegreesToCardinal(int degrees)
    {
        degrees = ((degrees % 360) + 360) % 360;
        string[] cardinals = { "N", "NNE", "NE", "ENE", "E", "ESE", "SE", "SSE", "S", "SSW", "SW", "WSW", "W", "WNW", "NW", "NNW" };
        int index = (int)Math.Round((double)degrees / 22.5) % 16;
        return cardinals[index];
    }
}
