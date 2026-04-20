using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Project list item response (lightweight)
/// </summary>
public class ProjectListResponse
{
    public Guid Id { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public string? OrgLogoUrl { get; set; }
    public string? ExpertName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public ProjectStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
