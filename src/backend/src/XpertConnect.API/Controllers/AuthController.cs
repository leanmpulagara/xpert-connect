using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Application.Features.Auth.DTOs;

namespace XpertConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
        _tokenService = tokenService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await _tokenService.RegisterAsync(
            request.Email,
            request.Password,
            request.FirstName,
            request.LastName);

        if (!result.Succeeded)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return Ok(MapToAuthResponse(result));
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var ipAddress = GetIpAddress();
        var result = await _tokenService.LoginAsync(request.Email, request.Password, ipAddress);

        if (!result.Succeeded)
        {
            return Unauthorized(new { errors = result.Errors });
        }

        return Ok(MapToAuthResponse(result));
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = GetIpAddress();
        var result = await _tokenService.RefreshTokenAsync(request.RefreshToken, ipAddress);

        if (!result.Succeeded)
        {
            return Unauthorized(new { errors = result.Errors });
        }

        return Ok(MapToAuthResponse(result));
    }

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    [Authorize]
    [HttpPost("revoke-token")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RevokeToken([FromBody] RefreshTokenRequest request)
    {
        var ipAddress = GetIpAddress();
        var result = await _tokenService.RevokeTokenAsync(request.RefreshToken, ipAddress, "Revoked by user");

        if (!result)
        {
            return BadRequest(new { message = "Token not found or already revoked." });
        }

        return Ok(new { message = "Token revoked successfully." });
    }

    /// <summary>
    /// Get current user information (protected endpoint)
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserInfo), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult GetCurrentUser()
    {
        var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        var firstName = User.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value;
        var lastName = User.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value;
        var roles = User.FindAll(System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);

        return Ok(new UserInfo
        {
            Id = userId ?? "",
            Email = email ?? "",
            FirstName = firstName ?? "",
            LastName = lastName ?? "",
            Roles = roles
        });
    }

    private static AuthResponse MapToAuthResponse(Application.Common.Models.AuthResult result)
    {
        return new AuthResponse
        {
            AccessToken = result.AccessToken!,
            RefreshToken = result.RefreshToken!,
            AccessTokenExpiration = result.AccessTokenExpiration!.Value,
            RefreshTokenExpiration = result.RefreshTokenExpiration!.Value,
            User = new UserInfo
            {
                Id = result.UserId!,
                Email = result.Email!,
                FirstName = result.FirstName!,
                LastName = result.LastName!,
                Roles = result.Roles ?? []
            }
        };
    }

    private string? GetIpAddress()
    {
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            return Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }
        return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
    }
}
