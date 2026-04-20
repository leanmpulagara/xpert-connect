using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Features.Escrow.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EscrowController : ControllerBase
{
    private readonly IEscrowRepository _escrowRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IPaymentService _paymentService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public EscrowController(
        IEscrowRepository escrowRepository,
        IPaymentRepository paymentRepository,
        IPaymentService paymentService,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _escrowRepository = escrowRepository;
        _paymentRepository = paymentRepository;
        _paymentService = paymentService;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Get escrow account by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(EscrowAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var escrow = await _escrowRepository.GetByIdAsync(id, cancellationToken);
        if (escrow == null)
        {
            return NotFound(new { message = "Escrow account not found" });
        }

        return Ok(_mapper.Map<EscrowAccountResponse>(escrow));
    }

    /// <summary>
    /// Create an escrow account for a payment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EscrowAccountResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateEscrowRequest request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, cancellationToken);
        if (payment == null)
        {
            return NotFound(new { message = "Payment not found" });
        }

        if (payment.Status != PaymentStatus.Authorized)
        {
            return BadRequest(new { message = "Payment must be authorized before creating escrow" });
        }

        // Check if escrow already exists
        var existingEscrow = await _escrowRepository.GetByPaymentIdAsync(request.PaymentId, cancellationToken);
        if (existingEscrow != null)
        {
            return BadRequest(new { message = "Escrow account already exists for this payment" });
        }

        // Create escrow account
        var escrow = new EscrowAccount
        {
            PaymentId = request.PaymentId,
            Amount = payment.Amount,
            Status = PaymentStatus.Pending,
            EscrowProviderRef = $"esc_{Guid.NewGuid():N}"
        };

        await _escrowRepository.CreateAsync(escrow, cancellationToken);

        // Add milestones if provided
        if (request.Milestones?.Any() == true)
        {
            foreach (var milestoneRequest in request.Milestones)
            {
                var milestone = new Milestone
                {
                    EscrowId = escrow.Id,
                    Description = milestoneRequest.Description,
                    Amount = milestoneRequest.Amount,
                    DueDate = milestoneRequest.DueDate?.ToUniversalTime()
                };
                await _escrowRepository.AddMilestoneAsync(milestone, cancellationToken);
            }
        }

        // Update payment status
        payment.Status = PaymentStatus.InEscrow;
        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        // Reload with milestones
        escrow = await _escrowRepository.GetByIdAsync(escrow.Id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = escrow!.Id }, _mapper.Map<EscrowAccountResponse>(escrow));
    }

    /// <summary>
    /// Fund an escrow account
    /// </summary>
    [HttpPost("{id:guid}/fund")]
    [ProducesResponseType(typeof(EscrowAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Fund(Guid id, [FromBody] FundEscrowRequest request, CancellationToken cancellationToken)
    {
        var escrow = await _escrowRepository.GetByIdAsync(id, cancellationToken);
        if (escrow == null)
        {
            return NotFound(new { message = "Escrow account not found" });
        }

        if (escrow.Status != PaymentStatus.Pending)
        {
            return BadRequest(new { message = "Escrow is not in pending status" });
        }

        // Update escrow status to funded
        escrow.Status = PaymentStatus.InEscrow;
        escrow.FundedAt = DateTime.UtcNow;
        await _escrowRepository.UpdateAsync(escrow, cancellationToken);

        return Ok(_mapper.Map<EscrowAccountResponse>(escrow));
    }

    /// <summary>
    /// Release escrow funds (all or by milestone)
    /// </summary>
    [HttpPost("{id:guid}/release")]
    [ProducesResponseType(typeof(EscrowAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Release(Guid id, [FromBody] ReleaseEscrowRequest request, CancellationToken cancellationToken)
    {
        var escrow = await _escrowRepository.GetByIdAsync(id, cancellationToken);
        if (escrow == null)
        {
            return NotFound(new { message = "Escrow account not found" });
        }

        if (escrow.Status != PaymentStatus.InEscrow)
        {
            return BadRequest(new { message = "Escrow is not funded" });
        }

        // If milestone specified, release just that milestone
        if (request.MilestoneId.HasValue)
        {
            var milestone = escrow.Milestones.FirstOrDefault(m => m.Id == request.MilestoneId.Value);
            if (milestone == null)
            {
                return NotFound(new { message = "Milestone not found" });
            }

            if (milestone.IsApproved)
            {
                return BadRequest(new { message = "Milestone already approved" });
            }

            await _escrowRepository.ApproveMilestoneAsync(milestone.Id, cancellationToken);

            // Check if all milestones approved
            escrow = await _escrowRepository.GetByIdAsync(id, cancellationToken);
            if (escrow!.Milestones.All(m => m.IsApproved))
            {
                escrow.Status = PaymentStatus.Released;
                escrow.ReleasedAt = DateTime.UtcNow;
                await _escrowRepository.UpdateAsync(escrow, cancellationToken);

                // Update payment status
                var payment = await _paymentRepository.GetByIdAsync(escrow.PaymentId, cancellationToken);
                if (payment != null)
                {
                    payment.Status = PaymentStatus.Released;
                    payment.CapturedAt = DateTime.UtcNow;
                    await _paymentRepository.UpdateAsync(payment, cancellationToken);
                }
            }
        }
        else
        {
            // Release all funds
            escrow.Status = PaymentStatus.Released;
            escrow.ReleasedAt = DateTime.UtcNow;
            await _escrowRepository.UpdateAsync(escrow, cancellationToken);

            // Approve all milestones
            foreach (var milestone in escrow.Milestones.Where(m => !m.IsApproved))
            {
                await _escrowRepository.ApproveMilestoneAsync(milestone.Id, cancellationToken);
            }

            // Update payment status
            var payment = await _paymentRepository.GetByIdAsync(escrow.PaymentId, cancellationToken);
            if (payment != null)
            {
                payment.Status = PaymentStatus.Released;
                payment.CapturedAt = DateTime.UtcNow;
                await _paymentRepository.UpdateAsync(payment, cancellationToken);
            }
        }

        // Reload
        escrow = await _escrowRepository.GetByIdAsync(id, cancellationToken);

        return Ok(_mapper.Map<EscrowAccountResponse>(escrow));
    }

    /// <summary>
    /// Dispute an escrow
    /// </summary>
    [HttpPost("{id:guid}/dispute")]
    [ProducesResponseType(typeof(EscrowAccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Dispute(Guid id, [FromBody] DisputeEscrowRequest request, CancellationToken cancellationToken)
    {
        var escrow = await _escrowRepository.GetByIdAsync(id, cancellationToken);
        if (escrow == null)
        {
            return NotFound(new { message = "Escrow account not found" });
        }

        if (escrow.Status != PaymentStatus.InEscrow)
        {
            return BadRequest(new { message = "Escrow cannot be disputed in current status" });
        }

        escrow.Status = PaymentStatus.Disputed;
        await _escrowRepository.UpdateAsync(escrow, cancellationToken);

        // Update payment status
        var payment = await _paymentRepository.GetByIdAsync(escrow.PaymentId, cancellationToken);
        if (payment != null)
        {
            payment.Status = PaymentStatus.Disputed;
            await _paymentRepository.UpdateAsync(payment, cancellationToken);
        }

        return Ok(_mapper.Map<EscrowAccountResponse>(escrow));
    }

    /// <summary>
    /// Add a milestone to an escrow account
    /// </summary>
    [HttpPost("{id:guid}/milestones")]
    [ProducesResponseType(typeof(MilestoneResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddMilestone(Guid id, [FromBody] CreateMilestoneRequest request, CancellationToken cancellationToken)
    {
        var escrow = await _escrowRepository.GetByIdAsync(id, cancellationToken);
        if (escrow == null)
        {
            return NotFound(new { message = "Escrow account not found" });
        }

        if (escrow.Status == PaymentStatus.Released || escrow.Status == PaymentStatus.Refunded)
        {
            return BadRequest(new { message = "Cannot add milestone to completed escrow" });
        }

        var milestone = new Milestone
        {
            EscrowId = id,
            Description = request.Description,
            Amount = request.Amount,
            DueDate = request.DueDate?.ToUniversalTime()
        };

        await _escrowRepository.AddMilestoneAsync(milestone, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id }, _mapper.Map<MilestoneResponse>(milestone));
    }

    /// <summary>
    /// Approve a milestone
    /// </summary>
    [HttpPost("milestones/{milestoneId:guid}/approve")]
    [ProducesResponseType(typeof(MilestoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ApproveMilestone(Guid milestoneId, CancellationToken cancellationToken)
    {
        var milestone = await _escrowRepository.GetMilestoneByIdAsync(milestoneId, cancellationToken);
        if (milestone == null)
        {
            return NotFound(new { message = "Milestone not found" });
        }

        if (milestone.IsApproved)
        {
            return BadRequest(new { message = "Milestone already approved" });
        }

        await _escrowRepository.ApproveMilestoneAsync(milestoneId, cancellationToken);

        // Reload
        milestone = await _escrowRepository.GetMilestoneByIdAsync(milestoneId, cancellationToken);

        return Ok(_mapper.Map<MilestoneResponse>(milestone));
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
