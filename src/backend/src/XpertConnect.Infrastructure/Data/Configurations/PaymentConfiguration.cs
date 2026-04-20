using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.Currency)
            .HasMaxLength(3);

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(p => p.StripePaymentId)
            .HasMaxLength(256);

        // Relationship: Payment -> EscrowAccount (one-to-one, optional)
        builder.HasOne(p => p.EscrowAccount)
            .WithOne(e => e.Payment)
            .HasForeignKey<EscrowAccount>(e => e.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
