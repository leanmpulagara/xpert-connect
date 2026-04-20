using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class ConsultationConfiguration : IEntityTypeConfiguration<Consultation>
{
    public void Configure(EntityTypeBuilder<Consultation> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Rate)
            .HasColumnType("decimal(18,2)");

        builder.Property(c => c.Currency)
            .HasMaxLength(3);

        builder.Property(c => c.VirtualHubLink)
            .HasMaxLength(1000);

        builder.Property(c => c.Notes)
            .HasMaxLength(5000);

        // Relationship: Consultation -> NDA (optional)
        builder.HasOne(c => c.Nda)
            .WithMany()
            .HasForeignKey(c => c.NdaId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationship: Consultation -> Payment (one-to-one)
        builder.HasOne(c => c.Payment)
            .WithOne(p => p.Consultation)
            .HasForeignKey<Payment>(p => p.ConsultationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Consultation -> PhysicalMeeting (one-to-one, optional)
        builder.HasOne(c => c.PhysicalMeeting)
            .WithOne(pm => pm.Consultation)
            .HasForeignKey<PhysicalMeeting>(pm => pm.ConsultationId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: Consultation -> Feedback (one-to-one, optional)
        builder.HasOne(c => c.Feedback)
            .WithOne(f => f.Consultation)
            .HasForeignKey<Feedback>(f => f.ConsultationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
