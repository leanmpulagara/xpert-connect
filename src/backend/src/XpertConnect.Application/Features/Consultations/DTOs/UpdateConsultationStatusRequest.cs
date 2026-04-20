using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Consultations.DTOs;

/// <summary>
/// Request to update consultation status
/// </summary>
public class UpdateConsultationStatusRequest
{
    public BookingStatus Status { get; set; }
    public string? Notes { get; set; }
}
