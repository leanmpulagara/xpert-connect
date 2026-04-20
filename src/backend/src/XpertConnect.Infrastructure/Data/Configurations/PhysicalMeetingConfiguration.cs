using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class PhysicalMeetingConfiguration : IEntityTypeConfiguration<PhysicalMeeting>
{
    public void Configure(EntityTypeBuilder<PhysicalMeeting> builder)
    {
        builder.HasKey(pm => pm.Id);

        // Relationship: PhysicalMeeting -> AuctionLot (optional, one-to-one)
        builder.HasOne(pm => pm.Auction)
            .WithOne(a => a.PhysicalMeeting)
            .HasForeignKey<PhysicalMeeting>(pm => pm.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: PhysicalMeeting -> Venue
        builder.HasOne(pm => pm.Venue)
            .WithMany(v => v.PhysicalMeetings)
            .HasForeignKey(pm => pm.VenueId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: PhysicalMeeting -> Geofence
        builder.HasOne(pm => pm.Geofence)
            .WithMany()
            .HasForeignKey(pm => pm.GeofenceId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: PhysicalMeeting -> Guests
        builder.HasMany(pm => pm.Guests)
            .WithOne(g => g.Meeting)
            .HasForeignKey(g => g.MeetingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
