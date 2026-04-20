using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Data.Configurations;

public class AuctionLotConfiguration : IEntityTypeConfiguration<AuctionLot>
{
    public void Configure(EntityTypeBuilder<AuctionLot> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(5000);

        builder.Property(a => a.StartingBid)
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.BuyNowPrice)
            .HasColumnType("decimal(18,2)");

        builder.Property(a => a.CurrentHighBid)
            .HasColumnType("decimal(18,2)");

        // Relationship: AuctionLot -> Expert
        builder.HasOne(a => a.Expert)
            .WithMany(e => e.AuctionLots)
            .HasForeignKey(a => a.ExpertId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: AuctionLot -> BeneficiaryOrg (optional)
        builder.HasOne(a => a.BeneficiaryOrg)
            .WithMany(n => n.BeneficiaryAuctions)
            .HasForeignKey(a => a.BeneficiaryOrgId)
            .OnDelete(DeleteBehavior.SetNull);

        // Relationship: AuctionLot -> Bids (one-to-many)
        builder.HasMany(a => a.Bids)
            .WithOne(b => b.Auction)
            .HasForeignKey(b => b.AuctionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship: AuctionLot -> WinningBid (one-to-one, optional)
        builder.HasOne(a => a.WinningBid)
            .WithOne()
            .HasForeignKey<AuctionLot>(a => a.WinningBidId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
