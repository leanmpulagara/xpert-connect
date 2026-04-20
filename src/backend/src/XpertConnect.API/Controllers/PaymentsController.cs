using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Payments.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly ISeekerRepository _seekerRepository;
    private readonly IPaymentService _paymentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public PaymentsController(
        IPaymentRepository paymentRepository,
        IConsultationRepository consultationRepository,
        ISeekerRepository seekerRepository,
        IPaymentService paymentService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _consultationRepository = consultationRepository;
        _seekerRepository = seekerRepository;
        _paymentService = paymentService;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Get payment by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        // Verify access
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isAdmin = User.IsInRole("Admin");
        var isOwner = payment.Consultation.Seeker.UserId == appUser.DomainUserId ||
                      payment.Consultation.Expert.UserId == appUser.DomainUserId;

        if (!isAdmin && !isOwner)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<PaymentResponse>(payment));
    }

    /// <summary>
    /// Get my payments
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(PagedResult<PaymentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPayments([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var result = await _paymentRepository.GetByUserIdAsync(appUser.DomainUserId.Value, page, pageSize, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<PaymentResponse>>(result.Items);

        return Ok(PagedResult<PaymentResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Authorize a payment for a consultation
    /// </summary>
    [HttpPost("authorize")]
    [Authorize(Policy = "RequireSeekerRole")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AuthorizePayment([FromBody] CreatePaymentRequest request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(request.ConsultationId, cancellationToken);
        if (consultation == null)
        {
            return NotFound(new { message = "Consultation not found" });
        }

        // Verify seeker owns the consultation
        var appUser = await GetCurrentAppUserAsync();
        var seeker = await _seekerRepository.GetByUserIdAsync(appUser!.DomainUserId!.Value, cancellationToken);
        if (seeker == null || consultation.SeekerId != seeker.Id)
        {
            return Forbid();
        }

        // Check if payment already exists
        var existingPayment = await _paymentRepository.GetByConsultationIdAsync(request.ConsultationId, cancellationToken);
        if (existingPayment != null)
        {
            return BadRequest(new { message = "Payment already exists for this consultation" });
        }

        // Calculate amount based on consultation rate and duration
        var amount = consultation.Rate * (consultation.DurationMinutes / 60m);

        // Create payment intent with Stripe
        var paymentIntent = await _paymentService.CreatePaymentIntentAsync(
            amount,
            "USD",
            request.PaymentMethodId,
            cancellationToken);

        if (!paymentIntent.IsSuccessful)
        {
            return BadRequest(new { message = paymentIntent.ErrorMessage ?? "Failed to create payment" });
        }

        // Create payment record
        var payment = new Payment
        {
            ConsultationId = request.ConsultationId,
            Amount = amount,
            Currency = "USD",
            PaymentMethod = request.PaymentMethodId,
            StripePaymentId = paymentIntent.PaymentIntentId,
            Status = PaymentStatus.Authorized,
            AuthorizedAt = DateTime.UtcNow
        };

        await _paymentRepository.CreateAsync(payment, cancellationToken);

        // Reload with navigation properties
        payment = await _paymentRepository.GetByIdAsync(payment.Id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = payment!.Id }, _mapper.Map<PaymentResponse>(payment));
    }

    /// <summary>
    /// Capture an authorized payment
    /// </summary>
    [HttpPost("{id:guid}/capture")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CapturePayment(Guid id, [FromBody] CapturePaymentRequest? request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        if (payment.Status != PaymentStatus.Authorized)
        {
            return BadRequest(new { message = "Payment is not in authorized status" });
        }

        // Verify access - only admin or consultation parties
        var appUser = await GetCurrentAppUserAsync();
        var isAdmin = User.IsInRole("Admin");
        var isOwner = payment.Consultation.Seeker.UserId == appUser!.DomainUserId ||
                      payment.Consultation.Expert.UserId == appUser.DomainUserId;

        if (!isAdmin && !isOwner)
        {
            return Forbid();
        }

        // Capture payment with Stripe
        var captureResult = await _paymentService.CapturePaymentAsync(
            payment.StripePaymentId!,
            request?.Amount,
            cancellationToken);

        if (!captureResult.IsSuccessful)
        {
            return BadRequest(new { message = captureResult.ErrorMessage ?? "Failed to capture payment" });
        }

        payment.Status = PaymentStatus.Released;
        payment.CapturedAt = DateTime.UtcNow;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return Ok(_mapper.Map<PaymentResponse>(payment));
    }

    /// <summary>
    /// Refund a payment
    /// </summary>
    [HttpPost("{id:guid}/refund")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefundPayment(Guid id, [FromBody] RefundPaymentRequest request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        if (payment.Status != PaymentStatus.Authorized && payment.Status != PaymentStatus.Released)
        {
            return BadRequest(new { message = "Payment cannot be refunded in current status" });
        }

        // Verify access - only admin can refund
        var isAdmin = User.IsInRole("Admin");
        if (!isAdmin)
        {
            return Forbid();
        }

        // Refund with Stripe
        var refundResult = await _paymentService.RefundPaymentAsync(
            payment.StripePaymentId!,
            request.Amount,
            request.Reason,
            cancellationToken);

        if (!refundResult.IsSuccessful)
        {
            return BadRequest(new { message = refundResult.ErrorMessage ?? "Failed to refund payment" });
        }

        payment.Status = PaymentStatus.Refunded;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return Ok(_mapper.Map<PaymentResponse>(payment));
    }

    /// <summary>
    /// Cancel an authorized payment
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CancelPayment(Guid id, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        if (payment.Status != PaymentStatus.Authorized && payment.Status != PaymentStatus.Pending)
        {
            return BadRequest(new { message = "Payment cannot be cancelled in current status" });
        }

        // Verify access
        var appUser = await GetCurrentAppUserAsync();
        var isAdmin = User.IsInRole("Admin");
        var isOwner = payment.Consultation.Seeker.UserId == appUser!.DomainUserId;

        if (!isAdmin && !isOwner)
        {
            return Forbid();
        }

        // Cancel with Stripe
        if (!string.IsNullOrEmpty(payment.StripePaymentId))
        {
            await _paymentService.CancelPaymentAsync(payment.StripePaymentId, cancellationToken);
        }

        payment.Status = PaymentStatus.Failed;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        return Ok(_mapper.Map<PaymentResponse>(payment));
    }

    private async Task<ApplicationUser?> GetCurrentAppUserAsync()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }
        return await _userManager.FindByIdAsync(userId);
    }
}
