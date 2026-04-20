namespace XpertConnect.Application.Features.Bids.DTOs;

/// <summary>
/// Request to place a bid on an auction
/// </summary>
public class PlaceBidRequest
{
    public decimal Amount { get; set; }
    public bool IsProxyBid { get; set; } = false;
    public decimal? MaxProxyAmount { get; set; }
}
