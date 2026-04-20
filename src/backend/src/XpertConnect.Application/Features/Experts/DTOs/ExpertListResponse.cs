using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Experts.DTOs;

/// <summary>
/// Simplified expert response for listings (public search)
/// </summary>
public class ExpertListResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    // User info
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string? ProfilePhotoUrl { get; set; }

    // Expert-specific info
    public ExpertCategory Category { get; set; }
    public string CategoryName => Category.ToString();
    public string? Headline { get; set; }
    public decimal HourlyRate { get; set; }
    public string? Currency { get; set; }
    public bool IsAvailable { get; set; }

    // Verification (for trust)
    public VerificationStatus VerificationStatus { get; set; }
    public string VerificationStatusName => VerificationStatus.ToString();

    // Stats
    public int CredentialCount { get; set; }
}
