namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// Payment service interface for payment provider abstraction (Stripe)
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Create a payment intent for authorization
    /// </summary>
    Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, string? paymentMethodId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirm/Capture a payment intent
    /// </summary>
    Task<PaymentIntentResult> CapturePaymentAsync(string paymentIntentId, decimal? amount = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refund a payment
    /// </summary>
    Task<RefundResult> RefundPaymentAsync(string paymentIntentId, decimal? amount = null, string? reason = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a payment intent
    /// </summary>
    Task<bool> CancelPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get payment intent status
    /// </summary>
    Task<PaymentIntentResult?> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default);
}

/// <summary>
/// Result of a payment intent operation
/// </summary>
public class PaymentIntentResult
{
    public string PaymentIntentId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string? ClientSecret { get; set; }
    public bool RequiresAction { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsSuccessful => string.IsNullOrEmpty(ErrorMessage);
}

/// <summary>
/// Result of a refund operation
/// </summary>
public class RefundResult
{
    public string RefundId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsSuccessful => string.IsNullOrEmpty(ErrorMessage);
}
