namespace XpertConnect.Application.Features.Bids.DTOs;

/// <summary>
/// Bid history for an auction (public view - anonymized)
/// </summary>
public class BidHistoryResponse
{
    public Guid AuctionId { get; set; }
    public int TotalBids { get; set; }
    public decimal? CurrentHighBid { get; set; }
    public IList<BidHistoryItem> RecentBids { get; set; } = new List<BidHistoryItem>();
}

public class BidHistoryItem
{
    public decimal Amount { get; set; }
    public DateTime PlacedAt { get; set; }
    public string BidderInitials { get; set; } = string.Empty;
    public bool IsCurrentHigh { get; set; }
}
