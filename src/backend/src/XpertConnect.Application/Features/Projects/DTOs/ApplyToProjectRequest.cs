namespace XpertConnect.Application.Features.Projects.DTOs;

/// <summary>
/// Request for an expert to apply to a pro-bono project
/// </summary>
public class ApplyToProjectRequest
{
    public string? CoverLetter { get; set; }
    public string? RelevantExperience { get; set; }
    public int? AvailableHoursPerWeek { get; set; }
}
