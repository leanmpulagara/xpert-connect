using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Auctions.DTOs;

/// <summary>
/// Full auction response
/// </summary>
public class AuctionResponse
{
    public Guid Id { get; set; }
    public Guid ExpertId { get; set; }
    public Guid? BeneficiaryOrgId { get; set; }

    // Expert info
    public string ExpertName { get; set; } = string.Empty;
    public string? ExpertProfilePhotoUrl { get; set; }
    public string? ExpertHeadline { get; set; }

    // Beneficiary info
    public string? BeneficiaryOrgName { get; set; }

    // Auction details
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MeetingType MeetingType { get; set; }
    public string MeetingTypeName => MeetingType.ToString();
    public int GuestLimit { get; set; }

    // Pricing
    public decimal StartingBid { get; set; }
    public decimal? BuyNowPrice { get; set; }
    public decimal? CurrentHighBid { get; set; }

    // Timing
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive => Status == AuctionStatus.Open && DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;
    public TimeSpan? TimeRemaining => IsActive ? EndTime - DateTime.UtcNow : null;

    // Status
    public AuctionStatus Status { get; set; }
    public string StatusName => Status.ToString();

    // Winning
    public Guid? WinningBidId { get; set; }
    public string? WinnerName { get; set; }

    // Stats
    public int TotalBids { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
