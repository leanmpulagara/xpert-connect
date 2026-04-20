namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Request to update a pro-bono project
/// </summary>
public class UpdateProjectRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Deliverables { get; set; }
    public int? EstimatedHours { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public List<string>? RequiredSkills { get; set; }
}
