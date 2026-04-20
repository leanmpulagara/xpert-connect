namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Request to create a new pro-bono project (NonProfit only)
/// </summary>
public class CreateProjectRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Deliverables { get; set; }
    public int EstimatedHours { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? RequiredSkills { get; set; }
}
