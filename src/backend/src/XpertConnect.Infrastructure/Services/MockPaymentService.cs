using XpertConnect.Application.Common.Interfaces;

namespace XpertConnect.Infrastructure.Services;

/// <summary>
/// Mock payment service for development/testing
/// In production, this would be replaced with StripePaymentService
/// </summary>
public class MockPaymentService : IPaymentService
{
    public Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, string? paymentMethodId = null, CancellationToken cancellationToken = default)
    {
        var result = new PaymentIntentResult
        {
            PaymentIntentId = $"pi_mock_{Guid.NewGuid():N}",
            Status = "requires_capture",
            Amount = amount,
            Currency = currency,
            ClientSecret = $"pi_mock_{Guid.NewGuid():N}_secret_{Guid.NewGuid():N}",
            RequiresAction = false
        };

        return Task.FromResult(result);
    }

    public Task<PaymentIntentResult> CapturePaymentAsync(string paymentIntentId, decimal? amount = null, CancellationToken cancellationToken = default)
    {
        var result = new PaymentIntentResult
        {
            PaymentIntentId = paymentIntentId,
            Status = "succeeded",
            Amount = amount ?? 0,
            Currency = "USD"
        };

        return Task.FromResult(result);
    }

    public Task<RefundResult> RefundPaymentAsync(string paymentIntentId, decimal? amount = null, string? reason = null, CancellationToken cancellationToken = default)
    {
        var result = new RefundResult
        {
            RefundId = $"re_mock_{Guid.NewGuid():N}",
            Status = "succeeded",
            Amount = amount ?? 0
        };

        return Task.FromResult(result);
    }

    public Task<bool> CancelPaymentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }

    public Task<PaymentIntentResult?> GetPaymentIntentAsync(string paymentIntentId, CancellationToken cancellationToken = default)
    {
        var result = new PaymentIntentResult
        {
            PaymentIntentId = paymentIntentId,
            Status = "requires_capture",
            Amount = 0,
            Currency = "USD"
        };

        return Task.FromResult<PaymentIntentResult?>(result);
    }
}
