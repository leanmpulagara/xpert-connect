using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// KYC (Know Your Customer) verification record
/// </summary>
public class KycVerification : AuditableEntity
{
    public Guid UserId { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? DocumentCountry { get; set; }
    public string? BiometricHash { get; set; }
    public VerificationStatus Status { get; set; } = VerificationStatus.Pending;
    public DateTime? VerifiedAt { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? ProviderRef { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
}
