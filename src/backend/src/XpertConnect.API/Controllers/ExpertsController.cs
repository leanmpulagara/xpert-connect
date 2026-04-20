using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Experts.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpertsController : ControllerBase
{
    private readonly IExpertRepository _expertRepository;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public ExpertsController(
        IExpertRepository expertRepository,
        IUserRepository userRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _expertRepository = expertRepository;
        _userRepository = userRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Search and list experts (public endpoint)
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ExpertListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] ExpertQueryParams queryParams, CancellationToken cancellationToken)
    {
        // Only show available and verified experts in public listing
        queryParams.IsAvailable ??= true;

        var result = await _expertRepository.GetAllAsync(queryParams, cancellationToken);
        var expertResponses = _mapper.Map<IReadOnlyList<ExpertListResponse>>(result.Items);

        return Ok(PagedResult<ExpertListResponse>.Create(
            expertResponses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Get expert by ID (public endpoint)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExpertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var expert = await _expertRepository.GetByIdAsync(id, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert not found" });
        }

        return Ok(_mapper.Map<ExpertResponse>(expert));
    }

    /// <summary>
    /// Get current user's expert profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(ExpertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found. Create one first." });
        }

        return Ok(_mapper.Map<ExpertResponse>(expert));
    }

    /// <summary>
    /// Create expert profile for current user
    /// </summary>
    [HttpPost("me")]
    [Authorize]
    [ProducesResponseType(typeof(ExpertResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMyProfile([FromBody] CreateExpertProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Create domain user if not exists
        if (appUser.DomainUserId == null)
        {
            var domainUser = new User
            {
                Email = appUser.Email ?? string.Empty,
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                PasswordHash = "managed-by-identity",
                UserType = UserType.Expert
            };
            await _userRepository.CreateAsync(domainUser, cancellationToken);

            appUser.DomainUserId = domainUser.Id;
            await _userManager.UpdateAsync(appUser);
        }

        // Check if already has expert profile
        var existingExpert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (existingExpert != null)
        {
            return BadRequest(new { message = "Expert profile already exists" });
        }

        // Update user type
        var domainUserToUpdate = await _userRepository.GetByIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (domainUserToUpdate != null)
        {
            domainUserToUpdate.UserType = UserType.Expert;
            await _userRepository.UpdateAsync(domainUserToUpdate, cancellationToken);
        }

        // Create expert profile
        var expert = new Expert
        {
            UserId = appUser.DomainUserId.Value,
            Category = request.Category,
            Headline = request.Headline,
            Bio = request.Bio,
            HourlyRate = request.HourlyRate,
            Currency = request.Currency,
            LinkedInUrl = request.LinkedInUrl,
            IsAvailable = true
        };

        await _expertRepository.CreateAsync(expert, cancellationToken);

        // Add Expert role
        if (!await _userManager.IsInRoleAsync(appUser, "Expert"))
        {
            await _userManager.AddToRoleAsync(appUser, "Expert");
        }

        // Reload with includes
        expert = await _expertRepository.GetByIdAsync(expert.Id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = expert!.Id }, _mapper.Map<ExpertResponse>(expert));
    }

    /// <summary>
    /// Update current user's expert profile
    /// </summary>
    [HttpPut("me")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(ExpertResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateExpertProfileRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        // Update only provided fields
        if (request.Category.HasValue)
            expert.Category = request.Category.Value;
        if (request.Headline != null)
            expert.Headline = request.Headline;
        if (request.Bio != null)
            expert.Bio = request.Bio;
        if (request.HourlyRate.HasValue)
            expert.HourlyRate = request.HourlyRate.Value;
        if (request.Currency != null)
            expert.Currency = request.Currency;
        if (request.IsAvailable.HasValue)
            expert.IsAvailable = request.IsAvailable.Value;
        if (request.LinkedInUrl != null)
            expert.LinkedInUrl = request.LinkedInUrl;
        if (request.SecurityLevel != null)
            expert.SecurityLevel = request.SecurityLevel;
        if (request.RequiresExecutiveProtection.HasValue)
            expert.RequiresExecutiveProtection = request.RequiresExecutiveProtection.Value;

        await _expertRepository.UpdateAsync(expert, cancellationToken);

        return Ok(_mapper.Map<ExpertResponse>(expert));
    }

    /// <summary>
    /// Add credential to expert profile
    /// </summary>
    [HttpPost("me/credentials")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(CredentialResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddCredential([FromBody] AddCredentialRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        var credential = new Credential
        {
            ExpertId = expert.Id,
            Type = request.Type,
            IssuingBody = request.IssuingBody,
            IssueDate = request.IssueDate.HasValue
                ? DateTime.SpecifyKind(request.IssueDate.Value, DateTimeKind.Utc)
                : null,
            ExpiryDate = request.ExpiryDate.HasValue
                ? DateTime.SpecifyKind(request.ExpiryDate.Value, DateTimeKind.Utc)
                : null,
            VerificationSource = request.VerificationSource,
            VerificationStatus = VerificationStatus.Pending
        };

        await _expertRepository.AddCredentialAsync(credential, cancellationToken);

        return CreatedAtAction(nameof(GetMyProfile), _mapper.Map<CredentialResponse>(credential));
    }

    /// <summary>
    /// Remove credential from expert profile
    /// </summary>
    [HttpDelete("me/credentials/{credentialId:guid}")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveCredential(Guid credentialId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        // Verify credential belongs to this expert
        if (!expert.Credentials.Any(c => c.Id == credentialId))
        {
            return NotFound(new { message = "Credential not found" });
        }

        var deleted = await _expertRepository.RemoveCredentialAsync(credentialId, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = "Credential not found" });
        }

        return Ok(new { message = "Credential removed successfully" });
    }

    /// <summary>
    /// Add availability slot
    /// </summary>
    [HttpPost("me/availability")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(typeof(AvailabilityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddAvailability([FromBody] AddAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        var availability = new ExpertAvailability
        {
            ExpertId = expert.Id,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            IsRecurring = request.IsRecurring,
            SpecificDate = request.SpecificDate
        };

        await _expertRepository.AddAvailabilityAsync(availability, cancellationToken);

        return CreatedAtAction(nameof(GetMyProfile), _mapper.Map<AvailabilityResponse>(availability));
    }

    /// <summary>
    /// Remove availability slot
    /// </summary>
    [HttpDelete("me/availability/{availabilityId:guid}")]
    [Authorize(Policy = "RequireExpertRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveAvailability(Guid availabilityId, CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser?.DomainUserId == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        var expert = await _expertRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (expert == null)
        {
            return NotFound(new { message = "Expert profile not found" });
        }

        // Verify availability belongs to this expert
        if (!expert.Availabilities.Any(a => a.Id == availabilityId))
        {
            return NotFound(new { message = "Availability slot not found" });
        }

        var deleted = await _expertRepository.RemoveAvailabilityAsync(availabilityId, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = "Availability slot not found" });
        }

        return Ok(new { message = "Availability slot removed successfully" });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
