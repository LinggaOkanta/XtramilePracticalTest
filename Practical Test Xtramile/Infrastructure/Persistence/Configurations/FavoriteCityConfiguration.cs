using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practical_Test_Xtramile.Domain.Entities;

namespace Practical_Test_Xtramile.Infrastructure.Persistence.Configurations;

public class FavoriteCityConfiguration : IEntityTypeConfiguration<FavoriteCity>
{
    public void Configure(EntityTypeBuilder<FavoriteCity> builder)
    {
        builder.ToTable("FavoriteCities");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.CityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(f => f.CountryCode)
            .IsRequired()
            .HasMaxLength(3);

        builder.HasIndex(f => new { f.CityName, f.CountryCode })
            .IsUnique();

        builder.Property(f => f.CreatedAtUtc)
            .IsRequired();
    }
}
