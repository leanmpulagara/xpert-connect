using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Consultations.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ConsultationsController : ControllerBase
{
    private readonly IConsultationRepository _consultationRepository;
    private readonly IExpertRepository _expertRepository;
    private readonly ISeekerRepository _seekerRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public ConsultationsController(
        IConsultationRepository consultationRepository,
        IExpertRepository expertRepository,
        ISeekerRepository seekerRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _consultationRepository = consultationRepository;
        _expertRepository = expertRepository;
        _seekerRepository = seekerRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Get consultation by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ConsultationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(id, cancellationToken);
        if (consultation == null)
        {
            return NotFound(new { message = "Consultation not found" });
        }

        // Verify user has access
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isAdmin = User.IsInRole("Admin");
        var isExpert = consultation.Expert.UserId == appUser.DomainUserId;
        var isSeeker = consultation.Seeker.UserId == appUser.DomainUserId;

        if (!isAdmin && !isExpert && !isSeeker)
        {
            return Forbid();
        }

        return Ok(_mapper.Map<ConsultationResponse>(consultation));
    }

    /// <summary>
    /// Get current user's consultations (as seeker)
    /// </summary>
    [HttpGet("my")]
    [Authorize(Policy = "RequireSeekerRole")]
    [ProducesResponseType(typeof(PagedResult<ConsultationListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyConsultations([FromQuery] ConsultationQueryParams queryParams, CancellationToken cancellationToken)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var seeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (seeker == null)
        {
            return NotFound(new { message = "Seeker profile not found" });
        }

        var result = await _consultationRepository.GetBySeekerIdAsync(seeker.Id, queryParams, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<ConsultationListResponse>>(result.Items);

        return Ok(PagedResult<ConsultationListResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Get expert's consultations
    /// </summary>
    [HttpGet("expert")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(PagedResult<ConsultationListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpertConsultations([FromQuery] ConsultationQueryParams queryParams, CancellationToken cancellationToken)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        var result = await _consultationRepository.GetByExpertIdAsync(expert.Id, queryParams, cancellationToken);
        var responses = _mapper.Map<IReadOnlyList<ConsultationListResponse>>(result.Items);

        return Ok(PagedResult<ConsultationListResponse>.Create(
            responses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Book a consultation with an expert
    /// </summary>
    [HttpPost]
    [Authorize(Policy = "RequireSeekerRole")]
    [ProducesResponseType(typeof(ConsultationResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create([FromBody] CreateConsultationRequest request, CancellationToken cancellationToken)
    {
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        // Get seeker profile
        var seeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (seeker == null)
        {
            return NotFound(new { message = "Seeker profile not found. Create one first." });
        }

        // Get expert
        var expert = await _expertRepository.GetByIdAsync(request.ExpertId, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert not found" });
        }

        if (!expert.IsAvailable)
        {
            return BadRequest(new { message = "Expert is not available for consultations" });
        }

        // Convert to UTC
        var scheduledAt = DateTime.SpecifyKind(request.ScheduledAt, DateTimeKind.Utc);

        // Check for overlapping bookings
        var hasOverlap = await _consultationRepository.HasOverlappingBookingAsync(
            expert.Id,
            scheduledAt,
            request.DurationMinutes,
            cancellationToken: cancellationToken);

        if (hasOverlap)
        {
            return BadRequest(new { message = "Expert has a conflicting booking at this time" });
        }

        // Create consultation
        var consultation = new Consultation
        {
            ExpertId = expert.Id,
            SeekerId = seeker.Id,
            ScheduledAt = scheduledAt,
            DurationMinutes = request.DurationMinutes,
            Rate = expert.HourlyRate,
            Currency = expert.Currency,
            Status = BookingStatus.Initiated,
            MeetingType = request.MeetingType,
            Notes = request.Notes
        };

        // Generate virtual meeting link for virtual meetings
        if (request.MeetingType == MeetingType.Virtual)
        {
            consultation.VirtualHubLink = $"https://meet.xpertconnect.com/{Guid.NewGuid():N}";
        }

        await _consultationRepository.CreateAsync(consultation, cancellationToken);

        // Reload with includes
        consultation = await _consultationRepository.GetByIdAsync(consultation.Id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = consultation!.Id }, _mapper.Map<ConsultationResponse>(consultation));
    }

    /// <summary>
    /// Update consultation status
    /// </summary>
    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(typeof(ConsultationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateConsultationStatusRequest request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(id, cancellationToken);
        if (consultation == null)
        {
            return NotFound(new { message = "Consultation not found" });
        }

        // Verify user has access
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isAdmin = User.IsInRole("Admin");
        var isExpert = consultation.Expert.UserId == appUser.DomainUserId;
        var isSeeker = consultation.Seeker.UserId == appUser.DomainUserId;

        if (!isAdmin && !isExpert && !isSeeker)
        {
            return Forbid();
        }

        // Validate status transition
        if (!IsValidStatusTransition(consultation.Status, request.Status, isExpert, isSeeker, isAdmin))
        {
            return BadRequest(new { message = $"Cannot transition from {consultation.Status} to {request.Status}" });
        }

        consultation.Status = request.Status;
        if (request.Notes != null)
        {
            consultation.Notes = string.IsNullOrEmpty(consultation.Notes)
                ? request.Notes
                : $"{consultation.Notes}\n---\n{request.Notes}";
        }

        if (request.Status == BookingStatus.Completed)
        {
            consultation.CompletedAt = DateTime.UtcNow;
        }

        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        return Ok(_mapper.Map<ConsultationResponse>(consultation));
    }

    /// <summary>
    /// Reschedule a consultation
    /// </summary>
    [HttpPost("{id:guid}/reschedule")]
    [ProducesResponseType(typeof(ConsultationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Reschedule(Guid id, [FromBody] RescheduleConsultationRequest request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(id, cancellationToken);
        if (consultation == null)
        {
            return NotFound(new { message = "Consultation not found" });
        }

        // Verify user has access
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isExpert = consultation.Expert.UserId == appUser.DomainUserId;
        var isSeeker = consultation.Seeker.UserId == appUser.DomainUserId;

        if (!isExpert && !isSeeker)
        {
            return Forbid();
        }

        // Can only reschedule certain statuses
        var reschedulableStatuses = new[]
        {
            BookingStatus.Initiated,
            BookingStatus.NdaSigned,
            BookingStatus.PaymentAuthorized,
            BookingStatus.Confirmed
        };

        if (!reschedulableStatuses.Contains(consultation.Status))
        {
            return BadRequest(new { message = $"Cannot reschedule consultation with status {consultation.Status}" });
        }

        var newScheduledAt = DateTime.SpecifyKind(request.NewScheduledAt, DateTimeKind.Utc);

        // Check for overlapping bookings
        var hasOverlap = await _consultationRepository.HasOverlappingBookingAsync(
            consultation.ExpertId,
            newScheduledAt,
            consultation.DurationMinutes,
            consultation.Id,
            cancellationToken);

        if (hasOverlap)
        {
            return BadRequest(new { message = "Expert has a conflicting booking at the new time" });
        }

        consultation.ScheduledAt = newScheduledAt;
        consultation.Status = BookingStatus.Rescheduled;
        if (!string.IsNullOrWhiteSpace(request.Reason))
        {
            consultation.Notes = string.IsNullOrEmpty(consultation.Notes)
                ? $"Rescheduled: {request.Reason}"
                : $"{consultation.Notes}\n---\nRescheduled: {request.Reason}";
        }

        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        return Ok(_mapper.Map<ConsultationResponse>(consultation));
    }

    /// <summary>
    /// Cancel a consultation
    /// </summary>
    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(typeof(ConsultationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id, [FromBody] string? reason, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(id, cancellationToken);
        if (consultation == null)
        {
            return NotFound(new { message = "Consultation not found" });
        }

        // Verify user has access
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var isAdmin = User.IsInRole("Admin");
        var isExpert = consultation.Expert.UserId == appUser.DomainUserId;
        var isSeeker = consultation.Seeker.UserId == appUser.DomainUserId;

        if (!isAdmin && !isExpert && !isSeeker)
        {
            return Forbid();
        }

        // Can't cancel completed/settled consultations
        var nonCancellableStatuses = new[]
        {
            BookingStatus.Completed,
            BookingStatus.Settled,
            BookingStatus.Cancelled,
            BookingStatus.Refunded
        };

        if (nonCancellableStatuses.Contains(consultation.Status))
        {
            return BadRequest(new { message = $"Cannot cancel consultation with status {consultation.Status}" });
        }

        consultation.Status = BookingStatus.Cancelled;
        if (!string.IsNullOrWhiteSpace(reason))
        {
            consultation.Notes = string.IsNullOrEmpty(consultation.Notes)
                ? $"Cancelled: {reason}"
                : $"{consultation.Notes}\n---\nCancelled: {reason}";
        }

        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        return Ok(_mapper.Map<ConsultationResponse>(consultation));
    }

    /// <summary>
    /// Mark consultation as completed
    /// </summary>
    [HttpPost("{id:guid}/complete")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(ConsultationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Complete(Guid id, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(id, cancellationToken);
        if (consultation == null)
        {
            return NotFound(new { message = "Consultation not found" });
        }

        // Verify expert owns this consultation
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null || consultation.Expert.UserId != appUser.DomainUserId)
        {
            return Forbid();
        }

        // Can only complete from certain statuses
        var completableStatuses = new[]
        {
            BookingStatus.Initiated,
            BookingStatus.Confirmed,
            BookingStatus.InProgress,
            BookingStatus.PendingCompletion
        };

        if (!completableStatuses.Contains(consultation.Status))
        {
            return BadRequest(new { message = $"Cannot complete consultation with status {consultation.Status}" });
        }

        consultation.Status = BookingStatus.Completed;
        consultation.CompletedAt = DateTime.UtcNow;

        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        return Ok(_mapper.Map<ConsultationResponse>(consultation));
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

    private static bool IsValidStatusTransition(BookingStatus current, BookingStatus next, bool isExpert, bool isSeeker, bool isAdmin)
    {
        // Admin can do anything
        if (isAdmin) return true;

        // Define valid transitions
        return (current, next) switch
        {
            // Expert can confirm from initiated/payment states
            (BookingStatus.Initiated, BookingStatus.Confirmed) when isExpert => true,
            (BookingStatus.NdaSigned, BookingStatus.Confirmed) when isExpert => true,
            (BookingStatus.PaymentAuthorized, BookingStatus.Confirmed) when isExpert => true,
            (BookingStatus.Rescheduled, BookingStatus.Confirmed) when isExpert => true,

            // Expert can progress the consultation
            (BookingStatus.Confirmed, BookingStatus.InProgress) when isExpert => true,
            (BookingStatus.InProgress, BookingStatus.PendingCompletion) when isExpert => true,
            (BookingStatus.InProgress, BookingStatus.Completed) when isExpert => true,
            (BookingStatus.PendingCompletion, BookingStatus.Completed) when isExpert => true,

            // Both can cancel
            (_, BookingStatus.Cancelled) when current != BookingStatus.Completed && current != BookingStatus.Settled => true,

            // Seeker can mark no-show
            (BookingStatus.Confirmed, BookingStatus.NoShow) when isSeeker => true,

            _ => false
        };
    }
}
