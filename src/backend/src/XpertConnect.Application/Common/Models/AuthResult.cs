namespace XpertConnect.Application.Common.Models;

/// <summary>
/// Authentication result containing tokens
/// </summary>
public class AuthResult
{
    public bool Succeeded { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpiration { get; set; }
    public DateTime? RefreshTokenExpiration { get; set; }
    public string? UserId { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public IEnumerable<string>? Roles { get; set; }
    public IEnumerable<string>? Errors { get; set; }

    public static AuthResult Success(
        string accessToken,
        string refreshToken,
        DateTime accessTokenExpiration,
        DateTime refreshTokenExpiration,
        string userId,
        string email,
        string firstName,
        string lastName,
        IEnumerable<string> roles)
    {
        return new AuthResult
        {
            Succeeded = true,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiration = accessTokenExpiration,
            RefreshTokenExpiration = refreshTokenExpiration,
            UserId = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Roles = roles
        };
    }

    public static AuthResult Failure(params string[] errors)
    {
        return new AuthResult
        {
            Succeeded = false,
            Errors = errors
        };
    }
}
