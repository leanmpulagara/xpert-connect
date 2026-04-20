using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class NdaConfiguration : IEntityTypeConfiguration<Nda>
{
    public void Configure(EntityTypeBuilder<Nda> builder)
    {
        builder.HasKey(n => n.Id);

        builder.Property(n => n.TemplateVersion)
            .HasMaxLength(20);

        builder.Property(n => n.DocumentUrl)
            .HasMaxLength(1000);

        // Relationship: NDA -> PartyA (User)
        builder.HasOne(n => n.PartyA)
            .WithMany()
            .HasForeignKey(n => n.PartyAId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: NDA -> PartyB (User)
        builder.HasOne(n => n.PartyB)
            .WithMany()
            .HasForeignKey(n => n.PartyBId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
