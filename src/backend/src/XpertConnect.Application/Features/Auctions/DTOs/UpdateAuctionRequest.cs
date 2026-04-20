using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Auctions.DTOs;

/// <summary>
/// Request to update an auction (only before it goes live)
/// </summary>
public class UpdateAuctionRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public MeetingType? MeetingType { get; set; }
    public int? GuestLimit { get; set; }
    public decimal? StartingBid { get; set; }
    public decimal? BuyNowPrice { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
}
