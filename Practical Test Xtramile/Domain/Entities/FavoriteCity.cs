using Practical_Test_Xtramile.Domain.Common;

namespace Practical_Test_Xtramile.Domain.Entities;

public class FavoriteCity : BaseEntity<Guid>
{
    public string CityName { get; private set; } = string.Empty;
    public string CountryCode { get; private set; } = string.Empty;
    public DateTime CreatedAtUtc { get; private set; }

    protected FavoriteCity() { }

    public FavoriteCity(Guid id, string cityName, string countryCode, DateTime createdAtUtc)
    {
        if (id == Guid.Empty) throw new ArgumentException("Id cannot be empty", nameof(id));
        if (string.IsNullOrWhiteSpace(cityName)) throw new ArgumentException("City name is required", nameof(cityName));
        if (string.IsNullOrWhiteSpace(countryCode)) throw new ArgumentException("Country code is required", nameof(countryCode));

        Id = id;
        CityName = cityName.Trim();
        CountryCode = countryCode.Trim().ToUpperInvariant();
        CreatedAtUtc = createdAtUtc;
    }
}
