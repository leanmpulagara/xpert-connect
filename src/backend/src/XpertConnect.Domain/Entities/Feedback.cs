using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Feedback/review for consultations
/// </summary>
public class Feedback : AuditableEntity
{
    public Guid ConsultationId { get; set; }
    public Guid SeekerId { get; set; }
    public int Rating { get; set; }
    public string? Comments { get; set; }

    // Navigation properties
    public virtual Consultation Consultation { get; set; } = null!;
    public virtual Seeker Seeker { get; set; } = null!;
}
