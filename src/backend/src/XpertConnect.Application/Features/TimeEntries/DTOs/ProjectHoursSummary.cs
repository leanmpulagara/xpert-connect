namespace XpertConnect.Application.Features.TimeEntries.DTOs;

/// <summary>
/// Summary of hours for CSR reporting
/// </summary>
public class ProjectHoursSummary
{
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public string OrgName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int EntryCount { get; set; }
    public DateTime? FirstEntry { get; set; }
    public DateTime? LastEntry { get; set; }
}

/// <summary>
/// Expert's CSR hours summary
/// </summary>
public class ExpertHoursSummary
{
    public Guid ExpertId { get; set; }
    public string ExpertName { get; set; } = string.Empty;
    public decimal TotalHours { get; set; }
    public int ProjectsCount { get; set; }
    public int CompletedProjectsCount { get; set; }
    public List<ProjectHoursSummary> Projects { get; set; } = new();
}
