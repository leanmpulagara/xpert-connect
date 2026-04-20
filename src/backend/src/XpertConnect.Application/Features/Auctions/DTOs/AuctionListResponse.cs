using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Auctions.DTOs;

/// <summary>
/// Simplified auction response for listings
/// </summary>
public class AuctionListResponse
{
    public Guid Id { get; set; }
    public Guid ExpertId { get; set; }

    // Expert info
    public string ExpertName { get; set; } = string.Empty;
    public string? ExpertProfilePhotoUrl { get; set; }

    // Auction details
    public string Title { get; set; } = string.Empty;
    public MeetingType MeetingType { get; set; }
    public string MeetingTypeName => MeetingType.ToString();

    // Pricing
    public decimal StartingBid { get; set; }
    public decimal? BuyNowPrice { get; set; }
    public decimal? CurrentHighBid { get; set; }

    // Timing
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive => Status == AuctionStatus.Open && DateTime.UtcNow >= StartTime && DateTime.UtcNow <= EndTime;

    // Status
    public AuctionStatus Status { get; set; }
    public string StatusName => Status.ToString();

    // Stats
    public int TotalBids { get; set; }
}
