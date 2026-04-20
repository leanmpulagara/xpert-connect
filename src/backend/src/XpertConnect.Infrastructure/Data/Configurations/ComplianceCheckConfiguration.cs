using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class ComplianceCheckConfiguration : IEntityTypeConfiguration<ComplianceCheck>
{
    public void Configure(EntityTypeBuilder<ComplianceCheck> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CheckType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Notes)
            .HasMaxLength(2000);

        // Relationship: ComplianceCheck -> User
        builder.HasOne(c => c.User)
            .WithMany(u => u.ComplianceChecks)
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
