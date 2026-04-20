using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Expert credentials (degrees, certifications, etc.)
/// </summary>
public class Credential : AuditableEntity
{
    public Guid ExpertId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string IssuingBody { get; set; } = string.Empty;
    public DateTime? IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? VerificationSource { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public DateTime? VerifiedAt { get; set; }

    // Navigation properties
    public virtual Expert Expert { get; set; } = null!;
}
