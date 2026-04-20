using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Memorandum of Understanding for pro-bono projects
/// </summary>
public class Mou : AuditableEntity
{
    public Guid ProjectId { get; set; }
    public string? Scope { get; set; }
    public string? Timeline { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? DocumentUrl { get; set; }

    // Navigation properties
    public virtual ProBonoProject Project { get; set; } = null!;
}
