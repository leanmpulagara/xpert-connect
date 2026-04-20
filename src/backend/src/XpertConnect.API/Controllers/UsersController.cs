using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Common.Models;
using XpertConnect.Application.Features.Users.DTOs;
using XpertConnect.Infrastructure.Identity;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;

    public UsersController(
        IUserRepository userRepository,
        UserManager<ApplicationUser> userManager,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all users (Admin only)
    /// </summary>
    [HttpGet]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(typeof(PagedResult<UserListResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] UserQueryParams queryParams, CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetAllAsync(queryParams, cancellationToken);
        var userResponses = _mapper.Map<IReadOnlyList<UserListResponse>>(result.Items);

        return Ok(PagedResult<UserListResponse>.Create(
            userResponses,
            result.TotalCount,
            result.PageNumber,
            result.PageSize));
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Check if current user can view this profile
        var currentUserId = GetCurrentUserId();
        var currentUserRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        if (currentUserId != id && !currentUserRoles.Contains("Admin"))
        {
            return Forbid();
        }

        return Ok(_mapper.Map<UserResponse>(user));
    }

    /// <summary>
    /// Get current user's profile
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        // Get from ApplicationUser to get the DomainUserId
        var appUser = await _userManager.FindByIdAsync(userId.ToString()!);
        if (appUser?.DomainUserId == null)
        {
            // Return basic info from Identity user if no domain user linked
            return Ok(new UserResponse
            {
                Id = appUser!.Id,
                Email = appUser.Email ?? "",
                FirstName = appUser.FirstName,
                LastName = appUser.LastName,
                IsActive = appUser.IsActive,
                LastLoginAt = appUser.LastLoginAt,
                CreatedAt = appUser.CreatedAt
            });
        }

        var user = await _userRepository.GetByIdAsync(appUser.DomainUserId.Value, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        return Ok(_mapper.Map<UserResponse>(user));
    }

    /// <summary>
    /// Update user profile
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        // Check authorization
        var currentUserId = GetCurrentUserId();
        var currentUserRoles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

        if (currentUserId != id && !currentUserRoles.Contains("Admin"))
        {
            return Forbid();
        }

        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Update only provided fields
        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName;

        if (request.Phone != null)
            user.Phone = request.Phone;

        if (request.ProfilePhotoUrl != null)
            user.ProfilePhotoUrl = request.ProfilePhotoUrl;

        await _userRepository.UpdateAsync(user, cancellationToken);

        // Also update ApplicationUser if names changed
        var appUser = await _userManager.FindByIdAsync(currentUserId.ToString()!);
        if (appUser != null)
        {
            if (!string.IsNullOrWhiteSpace(request.FirstName))
                appUser.FirstName = request.FirstName;
            if (!string.IsNullOrWhiteSpace(request.LastName))
                appUser.LastName = request.LastName;
            await _userManager.UpdateAsync(appUser);
        }

        return Ok(_mapper.Map<UserResponse>(user));
    }

    /// <summary>
    /// Update current user's profile
    /// </summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMe([FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
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

        // Update ApplicationUser
        if (!string.IsNullOrWhiteSpace(request.FirstName))
            appUser.FirstName = request.FirstName;
        if (!string.IsNullOrWhiteSpace(request.LastName))
            appUser.LastName = request.LastName;

        await _userManager.UpdateAsync(appUser);

        // Update domain user if linked
        if (appUser.DomainUserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(appUser.DomainUserId.Value, cancellationToken);
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(request.FirstName))
                    user.FirstName = request.FirstName;
                if (!string.IsNullOrWhiteSpace(request.LastName))
                    user.LastName = request.LastName;
                if (request.Phone != null)
                    user.Phone = request.Phone;
                if (request.ProfilePhotoUrl != null)
                    user.ProfilePhotoUrl = request.ProfilePhotoUrl;

                await _userRepository.UpdateAsync(user, cancellationToken);
                return Ok(_mapper.Map<UserResponse>(user));
            }
        }

        return Ok(new UserResponse
        {
            Id = appUser.Id,
            Email = appUser.Email ?? "",
            FirstName = appUser.FirstName,
            LastName = appUser.LastName,
            IsActive = appUser.IsActive,
            LastLoginAt = appUser.LastLoginAt,
            CreatedAt = appUser.CreatedAt
        });
    }

    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("me/change-password")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
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

        var result = await _userManager.ChangePasswordAsync(appUser, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "Password changed successfully" });
    }

    /// <summary>
    /// Delete user (Admin only)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _userRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound(new { message = "User not found" });
        }

        // Also deactivate ApplicationUser
        var appUser = await _userManager.FindByIdAsync(id.ToString());
        if (appUser != null)
        {
            appUser.IsActive = false;
            await _userManager.UpdateAsync(appUser);
        }

        return Ok(new { message = "User deleted successfully" });
    }

    /// <summary>
    /// Deactivate user (Admin only)
    /// </summary>
    [HttpPost("{id:guid}/deactivate")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.IsActive = false;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Also deactivate ApplicationUser
        var appUser = await _userManager.FindByIdAsync(id.ToString());
        if (appUser != null)
        {
            appUser.IsActive = false;
            await _userManager.UpdateAsync(appUser);
        }

        return Ok(new { message = "User deactivated successfully" });
    }

    /// <summary>
    /// Activate user (Admin only)
    /// </summary>
    [HttpPost("{id:guid}/activate")]
    [Authorize(Policy = "RequireAdminRole")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        user.IsActive = true;
        await _userRepository.UpdateAsync(user, cancellationToken);

        // Also activate ApplicationUser
        var appUser = await _userManager.FindByIdAsync(id.ToString());
        if (appUser != null)
        {
            appUser.IsActive = true;
            await _userManager.UpdateAsync(appUser);
        }

        return Ok(new { message = "User activated successfully" });
    }

    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
    }
}
