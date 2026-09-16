using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Practical_Test_Xtramile.Domain.Entities;

namespace Practical_Test_Xtramile.Infrastructure.Persistence.Configurations;

public class WeatherNoteConfiguration : IEntityTypeConfiguration<WeatherNote>
{
    public void Configure(EntityTypeBuilder<WeatherNote> builder)
    {
        builder.ToTable("WeatherNotes");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.CityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(w => w.Note)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(w => w.SkyConditions)
            .HasMaxLength(50);

        builder.Property(w => w.CreatedAtUtc)
            .IsRequired();
    }
}
