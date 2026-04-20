using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Escrow account for secure payment holding
/// </summary>
public class EscrowAccount : AuditableEntity
{
    public Guid PaymentId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? EscrowProviderRef { get; set; }
    public DateTime? FundedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }

    // Navigation properties
    public virtual Payment Payment { get; set; } = null!;
    public virtual ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
}
