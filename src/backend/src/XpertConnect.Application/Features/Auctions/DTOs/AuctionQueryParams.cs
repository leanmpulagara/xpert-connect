using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Auctions.DTOs;

/// <summary>
/// Query parameters for searching/filtering auctions
/// </summary>
public class AuctionQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public AuctionStatus? Status { get; set; }
    public MeetingType? MeetingType { get; set; }
    public Guid? ExpertId { get; set; }
    public bool? ActiveOnly { get; set; }
    public decimal? MinBid { get; set; }
    public decimal? MaxBid { get; set; }
    public string? SortBy { get; set; } = "EndTime";
    public bool SortDescending { get; set; } = false;
}
