using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Consultations.DTOs;

/// <summary>
/// Query parameters for filtering consultations
/// </summary>
public class ConsultationQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public BookingStatus? Status { get; set; }
    public MeetingType? MeetingType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public string? SortBy { get; set; } = "ScheduledAt";
    public bool SortDescending { get; set; } = true;
}
