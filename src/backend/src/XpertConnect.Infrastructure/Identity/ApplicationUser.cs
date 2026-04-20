using Microsoft.AspNetCore.Identity;
using XpertConnect.Domain.Entities;

namespace XpertConnect.Infrastructure.Identity;

/// <summary>
/// Application user for ASP.NET Identity authentication
/// Links to domain User entity for business logic
/// </summary>
public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Reference to the domain User entity
    /// </summary>
    public Guid? DomainUserId { get; set; }
    public virtual User? DomainUser { get; set; }

    /// <summary>
    /// Refresh tokens for JWT authentication
    /// </summary>
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}
