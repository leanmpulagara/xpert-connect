using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Query parameters for filtering/searching projects
/// </summary>
public class ProjectQueryParams
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public ProjectStatus? Status { get; set; }
    public Guid? OrgId { get; set; }
    public Guid? ExpertId { get; set; }
    public string? SearchTerm { get; set; }
    public bool? OpenForApplication { get; set; }
    public string? SortBy { get; set; } = "CreatedAt";
    public bool SortDescending { get; set; } = true;
}
