using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Experts.DTOs;

/// <summary>
/// Request to update expert profile
/// </summary>
public class UpdateExpertProfileRequest
{
    public ExpertCategory? Category { get; set; }
    public string? Headline { get; set; }
    public string? Bio { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? Currency { get; set; }
    public bool? IsAvailable { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? SecurityLevel { get; set; }
    public bool? RequiresExecutiveProtection { get; set; }
}
