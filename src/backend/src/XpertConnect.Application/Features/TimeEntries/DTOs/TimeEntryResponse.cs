namespace XpertConnect.Application.Features.TimeEntries.DTOs;

/// <summary>
/// Time entry response
/// </summary>
public class TimeEntryResponse
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string ProjectTitle { get; set; } = string.Empty;
    public Guid ExpertId { get; set; }
    public string ExpertName { get; set; } = string.Empty;
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public DateTime LoggedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
