using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Consultations.DTOs;

/// <summary>
/// Request to book a consultation with an expert
/// </summary>
public class CreateConsultationRequest
{
    public Guid ExpertId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; } = 60;
    public MeetingType MeetingType { get; set; } = MeetingType.Virtual;
    public string? Notes { get; set; }
}
