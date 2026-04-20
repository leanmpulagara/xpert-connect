using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Seeker profile - the person seeking expert access
/// </summary>
public class Seeker : AuditableEntity
{
    public Guid UserId { get; set; }
    public VerificationStatus KycStatus { get; set; } = VerificationStatus.Pending;
    public bool IsBidEligible { get; set; } = false;
    public bool IsPremium { get; set; } = false;
    public decimal ReputationScore { get; set; } = 0;
    public string? Company { get; set; }
    public string? JobTitle { get; set; }

    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual ICollection<FinancialReference> FinancialReferences { get; set; } = new List<FinancialReference>();
    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public virtual ICollection<Consultation> Consultations { get; set; } = new List<Consultation>();
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
}
