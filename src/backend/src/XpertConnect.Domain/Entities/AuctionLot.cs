using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Auction lot for high-value charity auctions (Warren Buffett model)
/// </summary>
public class AuctionLot : AuditableEntity
{
    public Guid ExpertId { get; set; }
    public Guid? BeneficiaryOrgId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MeetingType MeetingType { get; set; }
    public int GuestLimit { get; set; } = 0;
    public decimal StartingBid { get; set; }
    public decimal? BuyNowPrice { get; set; }
    public decimal? CurrentHighBid { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public AuctionStatus Status { get; set; } = AuctionStatus.Draft;
    public Guid? WinningBidId { get; set; }

    // Navigation properties
    public virtual Expert Expert { get; set; } = null!;
    public virtual NonProfitOrg? BeneficiaryOrg { get; set; }
    public virtual Bid? WinningBid { get; set; }
    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public virtual PhysicalMeeting? PhysicalMeeting { get; set; }
}
