using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Professional fee-based consultation booking
/// </summary>
public class Consultation : AuditableEntity
{
    public Guid ExpertId { get; set; }
    public Guid SeekerId { get; set; }
    public Guid? NdaId { get; set; }
    public DateTime ScheduledAt { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Rate { get; set; }
    public string? Currency { get; set; } = "USD";
    public BookingStatus Status { get; set; } = BookingStatus.Initiated;
    public MeetingType MeetingType { get; set; }
    public string? VirtualHubLink { get; set; }
    public string? Notes { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public virtual Expert Expert { get; set; } = null!;
    public virtual Seeker Seeker { get; set; } = null!;
    public virtual Nda? Nda { get; set; }
    public virtual Payment? Payment { get; set; }
    public virtual PhysicalMeeting? PhysicalMeeting { get; set; }
    public virtual Feedback? Feedback { get; set; }
}
