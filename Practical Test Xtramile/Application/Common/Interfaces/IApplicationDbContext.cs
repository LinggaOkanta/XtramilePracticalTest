using Microsoft.EntityFrameworkCore;
using Practical_Test_Xtramile.Domain.Entities;

namespace Practical_Test_Xtramile.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Country> Countries { get; }
    DbSet<City> Cities { get; }
    DbSet<WeatherNote> WeatherNotes { get; }
    DbSet<FavoriteCity> FavoriteCities { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
