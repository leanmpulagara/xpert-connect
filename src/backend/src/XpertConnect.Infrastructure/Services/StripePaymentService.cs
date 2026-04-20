using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using XpertConnect.Application.Common.Interfaces;

namespace XpertConnect.Infrastructure.Services;

/// <summary>
/// Stripe payment service implementation
/// </summary>
public class StripePaymentService : IPaymentService
{
    private readonly ILogger<StripePaymentService> _logger;
    private readonly PaymentIntentService _paymentIntentService;
    private readonly RefundService _refundService;

    public StripePaymentService(IConfiguration configuration, ILogger<StripePaymentService> logger)
    {
        _logger = logger;

        var apiKey = configuration["Stripe:SecretKey"]
            ?? throw new InvalidOperationException("Stripe:SecretKey not configured");

        StripeConfiguration.ApiKey = apiKey;

        _paymentIntentService = new PaymentIntentService();
        _refundService = new RefundService();
    }

    public async Task<PaymentIntentResult> CreatePaymentIntentAsync(
        decimal amount,
        string currency,
        string? paymentMethodId = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100), // Stripe uses cents
                Currency = currency.ToLower(),
                CaptureMethod = "manual", // Authorize only, capture later
                PaymentMethodTypes = new List<string> { "card" },
            };

            if (!string.IsNullOrEmpty(paymentMethodId))
            {
                options.PaymentMethod = paymentMethodId;
                options.Confirm = true;
            }

            var paymentIntent = await _paymentIntentService.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Created payment intent {PaymentIntentId} for amount {Amount} {Currency}",
                paymentIntent.Id, amount, currency);

            return new PaymentIntentResult
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = amount,
                Currency = currency,
                ClientSecret = paymentIntent.ClientSecret,
                RequiresAction = paymentIntent.Status == "requires_action"
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to create payment intent");
            return new PaymentIntentResult
            {
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<PaymentIntentResult> CapturePaymentAsync(
        string paymentIntentId,
        decimal? amount = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new PaymentIntentCaptureOptions();

            if (amount.HasValue)
            {
                options.AmountToCapture = (long)(amount.Value * 100);
            }

            var paymentIntent = await _paymentIntentService.CaptureAsync(
                paymentIntentId, options, cancellationToken: cancellationToken);

            _logger.LogInformation("Captured payment intent {PaymentIntentId}", paymentIntentId);

            return new PaymentIntentResult
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = paymentIntent.AmountReceived / 100m,
                Currency = paymentIntent.Currency.ToUpper()
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to capture payment intent {PaymentIntentId}", paymentIntentId);
            return new PaymentIntentResult
            {
                PaymentIntentId = paymentIntentId,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<RefundResult> RefundPaymentAsync(
        string paymentIntentId,
        decimal? amount = null,
        string? reason = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId,
            };

            if (amount.HasValue)
            {
                options.Amount = (long)(amount.Value * 100);
            }

            if (!string.IsNullOrEmpty(reason))
            {
                options.Reason = reason switch
                {
                    "duplicate" => "duplicate",
                    "fraudulent" => "fraudulent",
                    _ => "requested_by_customer"
                };
            }

            var refund = await _refundService.CreateAsync(options, cancellationToken: cancellationToken);

            _logger.LogInformation("Created refund {RefundId} for payment intent {PaymentIntentId}",
                refund.Id, paymentIntentId);

            return new RefundResult
            {
                RefundId = refund.Id,
                Status = refund.Status,
                Amount = refund.Amount / 100m
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to refund payment intent {PaymentIntentId}", paymentIntentId);
            return new RefundResult
            {
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<bool> CancelPaymentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _paymentIntentService.CancelAsync(paymentIntentId, cancellationToken: cancellationToken);

            _logger.LogInformation("Cancelled payment intent {PaymentIntentId}", paymentIntentId);

            return true;
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to cancel payment intent {PaymentIntentId}", paymentIntentId);
            return false;
        }
    }

    public async Task<PaymentIntentResult?> GetPaymentIntentAsync(
        string paymentIntentId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var paymentIntent = await _paymentIntentService.GetAsync(
                paymentIntentId, cancellationToken: cancellationToken);

            return new PaymentIntentResult
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = paymentIntent.Amount / 100m,
                Currency = paymentIntent.Currency.ToUpper(),
                ClientSecret = paymentIntent.ClientSecret,
                RequiresAction = paymentIntent.Status == "requires_action"
            };
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Failed to get payment intent {PaymentIntentId}", paymentIntentId);
            return null;
        }
    }
}
