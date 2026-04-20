using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Consultations.DTOs;

/// <summary>
/// Full consultation response
/// </summary>
public class ConsultationResponse
{
    public Guid Id { get; set; }
    public Guid ExpertId { get; set; }
    public Guid SeekerId { get; set; }

    // Expert info
    public string ExpertName { get; set; } = string.Empty;
    public string? ExpertProfilePhotoUrl { get; set; }

    // Seeker info
    public string SeekerName { get; set; } = string.Empty;
    public string? SeekerProfilePhotoUrl { get; set; }

    // Scheduling
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime ScheduledEndAt => ScheduledAt.AddMinutes(DurationMinutes);

    // Pricing
    public decimal Rate { get; set; }
    public string? Currency { get; set; }
    public decimal TotalAmount => Rate * (DurationMinutes / 60m);

    // Status
    public BookingStatus Status { get; set; }
    public string StatusName => Status.ToString();

    // Meeting details
    public MeetingType MeetingType { get; set; }
    public string MeetingTypeName => MeetingType.ToString();
    public string? VirtualHubLink { get; set; }
    public string? Notes { get; set; }

    // Completion
    public DateTime? CompletedAt { get; set; }
    public bool HasFeedback { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
