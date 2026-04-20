namespace XpertConnect.Application.Features.Payments.DTOs;

/// <summary>
/// Request to create/authorize a payment
/// </summary>
public class CreatePaymentRequest
{
    public Guid ConsultationId { get; set; }
    public string? PaymentMethodId { get; set; }
    public bool UseEscrow { get; set; } = false;
}

/// <summary>
/// Request to capture an authorized payment
/// </summary>
public class CapturePaymentRequest
{
    public decimal? Amount { get; set; }
}

/// <summary>
/// Request to refund a payment
/// </summary>
public class RefundPaymentRequest
{
    public decimal? Amount { get; set; }
    public string? Reason { get; set; }
}
