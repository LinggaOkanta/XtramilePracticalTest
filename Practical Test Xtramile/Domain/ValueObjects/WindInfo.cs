using Practical_Test_Xtramile.Domain.Enums;

namespace Practical_Test_Xtramile.Domain.ValueObjects;

public record WindInfo
{
    public double SpeedMph { get; init; }
    public int DirectionDegrees { get; init; }
    public CardinalDirection CardinalDirection { get; init; }
    public string DirectionCardinal { get; init; } = string.Empty;

    public static WindInfo Create(double speedMph, int directionDegrees)
    {
        var cardinal = DegreesToCardinal(directionDegrees);
        return new WindInfo
        {
            SpeedMph = Math.Round(speedMph, 2, MidpointRounding.AwayFromZero),
            DirectionDegrees = directionDegrees,
            CardinalDirection = cardinal,
            DirectionCardinal = cardinal.ToString()
        };
    }

    public static CardinalDirection DegreesToCardinal(int degrees)
    {
        degrees = ((degrees % 360) + 360) % 360;
        int index = (int)Math.Round((double)degrees / 22.5) % 16;
        return (CardinalDirection)index;
    }
}
