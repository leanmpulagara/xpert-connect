using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Features.Seekers.DTOs;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeekersController : ControllerBase
{
    private readonly ISeekerRepository _seekerRepository;
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public SeekersController(
        ISeekerRepository seekerRepository,
        IUserRepository userRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _seekerRepository = seekerRepository;
        _userRepository = userRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Get seeker by ID (Admin or self only)
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(SeekerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var seeker = await _seekerRepository.GetByIdAsync(id, cancellationToken);
        if (seeker == null)
        {
            return NotFound(new { message = "Seeker not found" });
        }

        // Check authorization - only admin or the seeker themselves
        var currentUserId = GetCurrentUserId();
        var currentUserRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        var appUser = await _userManager.FindByIdAsync(currentUserId.ToString()!);
        if (appUser?.DomainUserId != seeker.UserId && !currentUserRoles.Contains("Admin"))
        {
            return Forbid();
        }

        return Ok(_mapper.Map<SeekerResponse>(seeker));
    }

    /// <summary>
    /// Get current user's seeker profile
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(SeekerResponse), StatusCodes.Status200OK)]
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

        var seeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (seeker == null)
        {
            return NotFound(new { message = "Seeker profile not found. Create one first." });
        }

        return Ok(_mapper.Map<SeekerResponse>(seeker));
    }

    /// <summary>
    /// Create seeker profile for current user
    /// </summary>
    [HttpPost("me")]
    [ProducesResponseType(typeof(SeekerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateMyProfile([FromBody] CreateSeekerProfileRequest request, CancellationToken cancellationToken)
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
                UserType = UserType.Seeker
            };
            await _userRepository.CreateAsync(domainUser, cancellationToken);

            appUser.DomainUserId = domainUser.Id;
            await _userManager.UpdateAsync(appUser);
        }

        // Check if already has seeker profile
        var existingSeeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (existingSeeker != null)
        {
            return BadRequest(new { message = "Seeker profile already exists" });
        }

        // Update user type if not already set
        var domainUserToUpdate = await _userRepository.GetByIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (domainUserToUpdate != null && domainUserToUpdate.UserType != UserType.Expert)
        {
            domainUserToUpdate.UserType = UserType.Seeker;
            await _userRepository.UpdateAsync(domainUserToUpdate, cancellationToken);
        }

        // Create seeker profile
        var seeker = new Seeker
        {
            UserId = appUser.DomainUserId.Value,
            Company = request.Company,
            JobTitle = request.JobTitle,
            KycStatus = VerificationStatus.Pending,
            IsBidEligible = false,
            IsPremium = false,
            ReputationScore = 0
        };

        await _seekerRepository.CreateAsync(seeker, cancellationToken);

        // Add Seeker role
        if (!await _userManager.IsInRoleAsync(appUser, "Seeker"))
        {
            await _userManager.AddToRoleAsync(appUser, "Seeker");
        }

        // Reload with includes
        seeker = await _seekerRepository.GetByIdAsync(seeker.Id, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = seeker!.Id }, _mapper.Map<SeekerResponse>(seeker));
    }

    /// <summary>
    /// Update current user's seeker profile
    /// </summary>
    [HttpPut("me")]
    [Authorize(Policy = "RequireSeekerRole")]
    [ProducesResponseType(typeof(SeekerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateSeekerProfileRequest request, CancellationToken cancellationToken)
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

        var seeker = await _seekerRepository.GetByUserIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (seeker == null)
        {
            return NotFound(new { message = "Seeker profile not found" });
        }

        // Update only provided fields
        if (request.Company != null)
            seeker.Company = request.Company;
        if (request.JobTitle != null)
            seeker.JobTitle = request.JobTitle;

        await _seekerRepository.UpdateAsync(seeker, cancellationToken);

        return Ok(_mapper.Map<SeekerResponse>(seeker));
    }

    /// <summary>
    /// Delete seeker profile (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _seekerRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = "Seeker not found" });
        }

        return Ok(new { message = "Seeker profile deleted successfully" });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
