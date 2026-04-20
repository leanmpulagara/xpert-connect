using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Payment record for consultations and auctions
/// </summary>
public class Payment : AuditableEntity
{
    public Guid ConsultationId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string? PaymentMethod { get; set; }
    public string? StripePaymentId { get; set; }
    public DateTime? AuthorizedAt { get; set; }
    public DateTime? CapturedAt { get; set; }

    // Navigation properties
    public virtual Consultation Consultation { get; set; } = null!;
    public virtual EscrowAccount? EscrowAccount { get; set; }
}
