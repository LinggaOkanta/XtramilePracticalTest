using Practical_Test_Xtramile.Domain.Common;

namespace Practical_Test_Xtramile.Domain.Entities;

public class Country : BaseEntity<int>
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;

    private readonly List<City> _cities = new();
    public IReadOnlyCollection<City> Cities => _cities.AsReadOnly();

    // Required for EF Core
    protected Country() { }

    public Country(int id, string code, string name)
    {
        Id = id;
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void AddCity(City city)
    {
        ArgumentNullException.ThrowIfNull(city);
        _cities.Add(city);
    }
}
