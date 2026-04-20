using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Bid on an auction lot
/// </summary>
public class Bid : AuditableEntity
{
    public Guid AuctionId { get; set; }
    public Guid SeekerId { get; set; }
    public decimal Amount { get; set; }
    public bool IsProxyBid { get; set; } = false;
    public decimal? MaxProxyAmount { get; set; }
    public DateTime PlacedAt { get; set; } = DateTime.UtcNow;
    public bool IsWinning { get; set; } = false;

    // Navigation properties
    public virtual AuctionLot Auction { get; set; } = null!;
    public virtual Seeker Seeker { get; set; } = null!;
}
