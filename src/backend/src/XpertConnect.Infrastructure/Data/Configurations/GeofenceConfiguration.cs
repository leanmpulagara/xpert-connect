using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class GeofenceConfiguration : IEntityTypeConfiguration<Geofence>
{
    public void Configure(EntityTypeBuilder<Geofence> builder)
    {
        builder.HasKey(g => g.Id);

        builder.Property(g => g.BoundaryType)
            .HasMaxLength(20);

        builder.Property(g => g.CenterLatitude)
            .HasColumnType("decimal(10,7)");

        builder.Property(g => g.CenterLongitude)
            .HasColumnType("decimal(10,7)");

        builder.Property(g => g.PolygonCoords)
            .HasMaxLength(10000);

        // Relationship: Geofence -> GeofenceEvents
        builder.HasMany(g => g.GeofenceEvents)
            .WithOne(e => e.Geofence)
            .HasForeignKey(e => e.GeofenceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
