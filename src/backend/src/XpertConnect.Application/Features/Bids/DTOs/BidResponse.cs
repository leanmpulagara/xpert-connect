namespace XpertConnect.Application.Features.Bids.DTOs;

/// <summary>
/// Bid response
/// </summary>
public class BidResponse
{
    public Guid Id { get; set; }
    public Guid AuctionId { get; set; }
    public Guid SeekerId { get; set; }
    public string SeekerName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsProxyBid { get; set; }
    public decimal? MaxProxyAmount { get; set; }
    public DateTime PlacedAt { get; set; }
    public bool IsWinning { get; set; }
}
