using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Seekers.DTOs;

/// <summary>
/// Full seeker profile response
/// </summary>
public class SeekerResponse
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

    // Seeker-specific info
    public VerificationStatus KycStatus { get; set; }
    public string KycStatusName => KycStatus.ToString();
    public bool IsBidEligible { get; set; }
    public bool IsPremium { get; set; }
    public decimal ReputationScore { get; set; }
    public string? Company { get; set; }
    public string? JobTitle { get; set; }

    // User verification
    public VerificationStatus VerificationStatus { get; set; }
    public string VerificationStatusName => VerificationStatus.ToString();

    // Audit
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
