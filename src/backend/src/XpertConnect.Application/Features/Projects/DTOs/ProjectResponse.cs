using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Full project details response
/// </summary>
public class ProjectResponse
{
    public Guid Id { get; set; }
    public Guid OrgId { get; set; }
    public string OrgName { get; set; } = string.Empty;
    public string? OrgLogoUrl { get; set; }
    public Guid? ExpertId { get; set; }
    public string? ExpertName { get; set; }
    public string? ExpertProfilePhotoUrl { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Deliverables { get; set; }
    public int EstimatedHours { get; set; }
    public decimal ActualHours { get; set; }
    public decimal ProgressPercentage => EstimatedHours > 0
        ? Math.Min(100, Math.Round((ActualHours / EstimatedHours) * 100, 1))
        : 0;
    public ProjectStatus Status { get; set; }
    public string StatusName => Status.ToString();
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool HasMou { get; set; }
    public DateTime? MouSignedAt { get; set; }
    public int TimeEntriesCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
