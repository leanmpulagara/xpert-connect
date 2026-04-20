using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Time entry for pro-bono project tracking
/// </summary>
public class TimeEntry : AuditableEntity
{
    public Guid ProjectId { get; set; }
    public Guid ExpertId { get; set; }
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual ProBonoProject Project { get; set; } = null!;
    public virtual Expert Expert { get; set; } = null!;
}
