using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Experts.DTOs;

/// <summary>
/// Request to create an expert profile (user becomes an expert)
/// </summary>
public class CreateExpertProfileRequest
{
    public ExpertCategory Category { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public decimal HourlyRate { get; set; }
    public string Currency { get; set; } = "USD";
    public string? LinkedInUrl { get; set; }
}
