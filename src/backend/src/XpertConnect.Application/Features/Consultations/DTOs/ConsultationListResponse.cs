using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Consultations.DTOs;

/// <summary>
/// Simplified consultation response for listings
/// </summary>
public class ConsultationListResponse
{
    public Guid Id { get; set; }
    public Guid ExpertId { get; set; }
    public Guid SeekerId { get; set; }

    // Names
    public string ExpertName { get; set; } = string.Empty;
    public string SeekerName { get; set; } = string.Empty;

    // Scheduling
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }

    // Pricing
    public decimal Rate { get; set; }
    public string? Currency { get; set; }

    // Status
    public BookingStatus Status { get; set; }
    public string StatusName => Status.ToString();

    // Meeting type
    public MeetingType MeetingType { get; set; }
    public string MeetingTypeName => MeetingType.ToString();
}
