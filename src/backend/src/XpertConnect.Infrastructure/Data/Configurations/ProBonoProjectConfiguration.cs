using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class ProBonoProjectConfiguration : IEntityTypeConfiguration<ProBonoProject>
{
    public void Configure(EntityTypeBuilder<ProBonoProject> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(5000);

        builder.Property(p => p.Deliverables)
            .HasMaxLength(5000);

        builder.Property(p => p.ActualHours)
            .HasColumnType("decimal(10,2)");

        // Relationship: ProBonoProject -> NonProfitOrg
        builder.HasOne(p => p.Org)
            .WithMany(o => o.ProBonoProjects)
            .HasForeignKey(p => p.OrgId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: ProBonoProject -> Mou (one-to-one, optional)
        builder.HasOne(p => p.Mou)
            .WithOne(m => m.Project)
            .HasForeignKey<Mou>(m => m.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: ProBonoProject -> TimeEntries
        builder.HasMany(p => p.TimeEntries)
            .WithOne(te => te.Project)
            .HasForeignKey(te => te.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
