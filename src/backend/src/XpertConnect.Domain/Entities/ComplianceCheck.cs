using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Compliance check record (sanctions, PEP, adverse media)
/// </summary>
public class ComplianceCheck : AuditableEntity
{
    public Guid UserId { get; set; }
    public string CheckType { get; set; } = string.Empty;
    public bool SanctionsClear { get; set; } = false;
    public bool PepClear { get; set; } = false;
    public bool AdverseMediaClear { get; set; } = false;
    public DateTime CheckedAt { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
