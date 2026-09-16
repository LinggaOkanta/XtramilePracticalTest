using Practical_Test_Xtramile.Domain.Common;

namespace Practical_Test_Xtramile.Domain.Entities;

public class City : BaseEntity<int>
{
    public int CountryId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public Country? Country { get; private set; }

    protected City() { }

    public City(int id, int countryId, string name)
    {
        Id = id;
        CountryId = countryId;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }
}
