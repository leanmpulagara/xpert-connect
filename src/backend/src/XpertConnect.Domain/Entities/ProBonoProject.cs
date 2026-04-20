using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Pro-bono project for social impact
/// </summary>
public class ProBonoProject : AuditableEntity
{
    public Guid OrgId { get; set; }
    public Guid? ExpertId { get; set; }
    public Guid? MouId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Deliverables { get; set; }
    public int EstimatedHours { get; set; }
    public decimal ActualHours { get; set; } = 0;
    public ProjectStatus Status { get; set; } = ProjectStatus.Draft;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    // Navigation properties
    public virtual NonProfitOrg Org { get; set; } = null!;
    public virtual Expert? Expert { get; set; }
    public virtual Mou? Mou { get; set; }
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
