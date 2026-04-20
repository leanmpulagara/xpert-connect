using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class ExpertConfiguration : IEntityTypeConfiguration<Expert>
{
    public void Configure(EntityTypeBuilder<Expert> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Headline)
            .HasMaxLength(500);

        builder.Property(e => e.Bio)
            .HasMaxLength(5000);

        builder.Property(e => e.HourlyRate)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.Currency)
            .HasMaxLength(3);

        builder.Property(e => e.LinkedInUrl)
            .HasMaxLength(500);

        builder.Property(e => e.SecurityLevel)
            .HasMaxLength(50);

        // Relationship: Expert -> Credentials
        builder.HasMany(e => e.Credentials)
            .WithOne(c => c.Expert)
            .HasForeignKey(c => c.ExpertId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Expert -> Consultations
        builder.HasMany(e => e.Consultations)
            .WithOne(c => c.Expert)
            .HasForeignKey(c => c.ExpertId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Expert -> ProBonoProjects
        builder.HasMany(e => e.ProBonoProjects)
            .WithOne(p => p.Expert)
            .HasForeignKey(p => p.ExpertId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Expert -> Availabilities
        builder.HasMany(e => e.Availabilities)
            .WithOne(a => a.Expert)
            .HasForeignKey(a => a.ExpertId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
