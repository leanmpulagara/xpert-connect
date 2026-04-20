using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class GeofenceEventConfiguration : IEntityTypeConfiguration<GeofenceEvent>
{
    public void Configure(EntityTypeBuilder<GeofenceEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Latitude)
            .HasColumnType("decimal(10,7)");

        builder.Property(e => e.Longitude)
            .HasColumnType("decimal(10,7)");

        // Relationship: GeofenceEvent -> User
        builder.HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
