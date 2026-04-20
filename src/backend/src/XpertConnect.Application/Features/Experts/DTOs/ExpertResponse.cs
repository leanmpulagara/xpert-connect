using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Experts.DTOs;

/// <summary>
/// Full expert profile response with credentials and availability
/// </summary>
public class ExpertResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    // User info
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? Phone { get; set; }
    public string? ProfilePhotoUrl { get; set; }

    // Expert-specific info
    public ExpertCategory Category { get; set; }
    public string CategoryName => Category.ToString();
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public string? Currency { get; set; }
    public bool IsAvailable { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? SecurityLevel { get; set; }
    public bool RequiresExecutiveProtection { get; set; }

    // Verification
    public VerificationStatus VerificationStatus { get; set; }
    public string VerificationStatusName => VerificationStatus.ToString();

    // Related data
    public IList<CredentialResponse> Credentials { get; set; } = new List<CredentialResponse>();
    public IList<AvailabilityResponse> Availabilities { get; set; } = new List<AvailabilityResponse>();

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
