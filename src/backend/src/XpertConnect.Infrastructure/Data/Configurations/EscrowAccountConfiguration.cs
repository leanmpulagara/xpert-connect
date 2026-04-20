using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class EscrowAccountConfiguration : IEntityTypeConfiguration<EscrowAccount>
{
    public void Configure(EntityTypeBuilder<EscrowAccount> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.EscrowProviderRef)
            .HasMaxLength(256);

        // Relationship: EscrowAccount -> Milestones
        builder.HasMany(e => e.Milestones)
            .WithOne(m => m.Escrow)
            .HasForeignKey(m => m.EscrowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
