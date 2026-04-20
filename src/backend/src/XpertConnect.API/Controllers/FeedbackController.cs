using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Feedback.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api")]
public class FeedbackController : ControllerBase
{
    private readonly IFeedbackRepository _feedbackRepository;
    private readonly IConsultationRepository _consultationRepository;
    private readonly ISeekerRepository _seekerRepository;
    private readonly IExpertRepository _expertRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public FeedbackController(
        IFeedbackRepository feedbackRepository,
        IConsultationRepository consultationRepository,
        ISeekerRepository seekerRepository,
        IExpertRepository expertRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _feedbackRepository = feedbackRepository;
        _consultationRepository = consultationRepository;
        _seekerRepository = seekerRepository;
        _expertRepository = expertRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Submit feedback for a completed consultation
    /// </summary>
    [HttpPost("consultations/{consultationId:guid}/feedback")]
    [Authorize(Policy = "RequireSeekerRole")]
    [ProducesResponseType(typeof(FeedbackResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFeedback(Guid consultationId, [FromBody] CreateFeedbackRequest request, CancellationToken cancellationToken)
    {
        var consultation = await _consultationRepository.GetByIdAsync(consultationId, cancellationToken);
        if (consultation == null)
        {
            return NotFound(new { message = "Consultation not found" });
        }

        // Verify seeker owns this consultation
        var appUser = await GetCurrentAppUserAsync();
        if (appUser?.DomainUserId == null)
        {
            return Forbid();
        }

        var seeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (seeker == null || consultation.SeekerId != seeker.Id)
        {
            return Forbid();
        }

        // Can only provide feedback for completed consultations
        if (consultation.Status != BookingStatus.Completed && consultation.Status != BookingStatus.FeedbackPending)
        {
            return BadRequest(new { message = "Can only provide feedback for completed consultations" });
        }

        // Check if feedback already exists
        var existingFeedback = await _feedbackRepository.GetByConsultationIdAsync(consultationId, cancellationToken);
        if (existingFeedback != null)
        {
            return BadRequest(new { message = "Feedback already submitted for this consultation" });
        }

        var feedback = new Feedback
        {
            ConsultationId = consultationId,
            SeekerId = seeker.Id,
            Rating = request.Rating,
            Comments = request.Comments
        };

        await _feedbackRepository.CreateAsync(feedback, cancellationToken);

        // Update consultation status
        consultation.Status = BookingStatus.Settled;
        await _consultationRepository.UpdateAsync(consultation, cancellationToken);

        // Reload with includes
        feedback = await _feedbackRepository.GetByIdAsync(feedback.Id, cancellationToken);
        return CreatedAtAction(nameof(GetFeedback), new { consultationId }, _mapper.Map<FeedbackResponse>(feedback));
    }

    /// <summary>
    /// Get feedback for a consultation
    /// </summary>
    [HttpGet("consultations/{consultationId:guid}/feedback")]
    [ProducesResponseType(typeof(FeedbackResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetFeedback(Guid consultationId, CancellationToken cancellationToken)
    {
        var feedback = await _feedbackRepository.GetByConsultationIdAsync(consultationId, cancellationToken);
        if (feedback == null)
        {
            return NotFound(new { message = "Feedback not found" });
        }

        return Ok(_mapper.Map<FeedbackResponse>(feedback));
    }

    /// <summary>
    /// Get expert's feedback summary and reviews (public)
    /// </summary>
    [HttpGet("experts/{expertId:guid}/feedback")]
    [ProducesResponseType(typeof(ExpertFeedbackSummary), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExpertFeedback(Guid expertId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var expert = await _expertRepository.GetByIdAsync(expertId, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert not found" });
        }

        var (averageRating, totalCount) = await _feedbackRepository.GetExpertRatingSummaryAsync(expertId, cancellationToken);
        var distribution = await _feedbackRepository.GetExpertRatingDistributionAsync(expertId, cancellationToken);
        var feedbackPage = await _feedbackRepository.GetByExpertIdAsync(expertId, page, pageSize, cancellationToken);

        var summary = new ExpertFeedbackSummary
        {
            ExpertId = expertId,
            AverageRating = Math.Round(averageRating, 1),
            TotalReviews = totalCount,
            FiveStarCount = distribution[5],
            FourStarCount = distribution[4],
            ThreeStarCount = distribution[3],
            TwoStarCount = distribution[2],
            OneStarCount = distribution[1],
            RecentFeedback = _mapper.Map<IList<FeedbackResponse>>(feedbackPage.Items)
        };

        return Ok(summary);
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
