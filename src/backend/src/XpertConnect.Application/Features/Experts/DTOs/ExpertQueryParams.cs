using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Experts.DTOs;

/// <summary>
/// Query parameters for searching/filtering experts
/// </summary>
public class ExpertQueryParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public ExpertCategory? Category { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
    public bool? IsAvailable { get; set; }
    public decimal? MinHourlyRate { get; set; }
    public decimal? MaxHourlyRate { get; set; }
    public string? Currency { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}
