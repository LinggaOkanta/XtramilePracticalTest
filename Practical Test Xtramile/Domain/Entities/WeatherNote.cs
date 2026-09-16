using Practical_Test_Xtramile.Domain.Common;

namespace Practical_Test_Xtramile.Domain.Entities;

public class WeatherNote : BaseEntity<Guid>
{
    public string CityName { get; private set; } = string.Empty;
    public string Note { get; private set; } = string.Empty;
    public double? TemperatureCelsius { get; private set; }
    public string? SkyConditions { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    protected WeatherNote() { }

    public WeatherNote(Guid id, string cityName, string note, double? temperatureCelsius, string? skyConditions, DateTime createdAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty", nameof(id));
        if (string.IsNullOrWhiteSpace(cityName)) throw new ArgumentException("City name is required", nameof(cityName));
        if (string.IsNullOrWhiteSpace(note)) throw new ArgumentException("Note cannot be empty", nameof(note));

        Id = id;
        CityName = cityName.Trim();
        Note = note.Trim();
        TemperatureCelsius = temperatureCelsius;
        SkyConditions = skyConditions?.Trim();
        CreatedAtUtc = createdAtUtc;
    }
}
