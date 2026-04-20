namespace XpertConnect.Application.Features.Consultations.DTOs;

/// <summary>
/// Request to reschedule a consultation
/// </summary>
public class RescheduleConsultationRequest
{
    public DateTime NewScheduledAt { get; set; }
    public string? Reason { get; set; }
}
