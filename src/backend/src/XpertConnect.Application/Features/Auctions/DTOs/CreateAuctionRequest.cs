using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Auctions.DTOs;

/// <summary>
/// Request to create an auction
/// </summary>
public class CreateAuctionRequest
{
    public Guid? BeneficiaryOrgId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public MeetingType MeetingType { get; set; } = MeetingType.InPerson_Lunch;
    public int GuestLimit { get; set; } = 0;
    public decimal StartingBid { get; set; }
    public decimal? BuyNowPrice { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
