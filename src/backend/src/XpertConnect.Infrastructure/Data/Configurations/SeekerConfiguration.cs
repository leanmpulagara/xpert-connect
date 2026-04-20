using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class SeekerConfiguration : IEntityTypeConfiguration<Seeker>
{
    public void Configure(EntityTypeBuilder<Seeker> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ReputationScore)
            .HasColumnType("decimal(5,2)");

        builder.Property(s => s.Company)
            .HasMaxLength(256);

        builder.Property(s => s.JobTitle)
            .HasMaxLength(256);

        // Relationship: Seeker -> FinancialReferences
        builder.HasMany(s => s.FinancialReferences)
            .WithOne(f => f.Seeker)
            .HasForeignKey(f => f.SeekerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Seeker -> Bids
        builder.HasMany(s => s.Bids)
            .WithOne(b => b.Seeker)
            .HasForeignKey(b => b.SeekerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Seeker -> Consultations
        builder.HasMany(s => s.Consultations)
            .WithOne(c => c.Seeker)
            .HasForeignKey(c => c.SeekerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Seeker -> Feedbacks
        builder.HasMany(s => s.Feedbacks)
            .WithOne(f => f.Seeker)
            .HasForeignKey(f => f.SeekerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
