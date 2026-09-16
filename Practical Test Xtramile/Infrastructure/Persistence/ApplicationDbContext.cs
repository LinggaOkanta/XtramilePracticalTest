using Microsoft.EntityFrameworkCore;
using Practical_Test_Xtramile.Application.Common.Interfaces;
using Practical_Test_Xtramile.Domain.Entities;

namespace Practical_Test_Xtramile.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<WeatherNote> WeatherNotes => Set<WeatherNote>();
    public DbSet<FavoriteCity> FavoriteCities => Set<FavoriteCity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
