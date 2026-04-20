using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Base user entity for all user types
/// </summary>
public class User : AuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public UserType UserType { get; set; }
    public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public virtual Expert? Expert { get; set; }
    public virtual Seeker? Seeker { get; set; }
    public virtual NonProfitOrg? NonProfitOrg { get; set; }
    public virtual ICollection<KycVerification> KycVerifications { get; set; } = new List<KycVerification>();
    public virtual ICollection<ComplianceCheck> ComplianceChecks { get; set; } = new List<ComplianceCheck>();
}
