using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Non-Disclosure Agreement for consultations
/// </summary>
public class Nda : AuditableEntity
{
    public Guid PartyAId { get; set; }
    public Guid PartyBId { get; set; }
    public string TemplateVersion { get; set; } = "1.0";
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PartyASignedAt { get; set; }
    public DateTime? PartyBSignedAt { get; set; }
    public string? DocumentUrl { get; set; }

    // Navigation properties
    public virtual User PartyA { get; set; } = null!;
    public virtual User PartyB { get; set; } = null!;
    public virtual Consultation? Consultation { get; set; }
}
