using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class VenueConfiguration : IEntityTypeConfiguration<Venue>
{
    public void Configure(EntityTypeBuilder<Venue> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(v => v.Address)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(v => v.City)
            .HasMaxLength(100);

        builder.Property(v => v.Country)
            .HasMaxLength(100);

        builder.Property(v => v.Latitude)
            .HasColumnType("decimal(10,7)");

        builder.Property(v => v.Longitude)
            .HasColumnType("decimal(10,7)");

        builder.Property(v => v.SecurityRating)
            .HasMaxLength(50);

        builder.Property(v => v.Amenities)
            .HasMaxLength(2000);

        // Relationship: Venue -> Geofence (one-to-one)
        builder.HasOne(v => v.Geofence)
            .WithOne(g => g.Venue)
            .HasForeignKey<Geofence>(g => g.VenueId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
