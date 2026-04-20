using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Hours)
            .HasColumnType("decimal(5,2)");

        builder.Property(t => t.Description)
            .HasMaxLength(2000);

        // Relationship: TimeEntry -> Expert
        builder.HasOne(t => t.Expert)
            .WithMany()
            .HasForeignKey(t => t.ExpertId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
