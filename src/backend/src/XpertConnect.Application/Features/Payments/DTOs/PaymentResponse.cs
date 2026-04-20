using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Payments.DTOs;

/// <summary>
/// Payment details response
/// </summary>
public class PaymentResponse
{
    public Guid Id { get; set; }
    public Guid ConsultationId { get; set; }
    public string? ConsultationTitle { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public string? PaymentMethod { get; set; }
    public string? StripePaymentId { get; set; }
    public DateTime? AuthorizedAt { get; set; }
    public DateTime? CapturedAt { get; set; }
    public bool HasEscrow { get; set; }
    public Guid? EscrowAccountId { get; set; }
    public DateTime CreatedAt { get; set; }
}
