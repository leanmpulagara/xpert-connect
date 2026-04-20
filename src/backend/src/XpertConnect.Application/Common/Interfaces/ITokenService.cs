using XpertConnect.Application.Common.Models;

namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// Service for JWT token generation and validation
/// </summary>
public interface ITokenService
{
    /// <summary>
    /// Register a new user
    /// </summary>
    Task<AuthResult> RegisterAsync(string email, string password, string firstName, string lastName);

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    Task<AuthResult> LoginAsync(string email, string password, string? ipAddress = null);

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    Task<AuthResult> RefreshTokenAsync(string refreshToken, string? ipAddress = null);

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null, string? reason = null);

    /// <summary>
    /// Revoke all refresh tokens for a user
    /// </summary>
    Task<bool> RevokeAllTokensAsync(Guid userId, string? ipAddress = null, string? reason = null);
}
