using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class KycVerificationConfiguration : IEntityTypeConfiguration<KycVerification>
{
    public void Configure(EntityTypeBuilder<KycVerification> builder)
    {
        builder.HasKey(k => k.Id);

        builder.Property(k => k.DocumentType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(k => k.DocumentCountry)
            .HasMaxLength(100);

        builder.Property(k => k.BiometricHash)
            .HasMaxLength(500);

        builder.Property(k => k.ProviderRef)
            .HasMaxLength(256);

        // Relationship: KycVerification -> User
        builder.HasOne(k => k.User)
            .WithMany(u => u.KycVerifications)
            .HasForeignKey(k => k.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
