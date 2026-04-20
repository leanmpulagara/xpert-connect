namespace XpertConnect.Application.Features.TimeEntries.DTOs;

/// <summary>
/// Request to log time for a pro-bono project
/// </summary>
public class CreateTimeEntryRequest
{
    public decimal Hours { get; set; }
    public string? Description { get; set; }
    public DateTime? LoggedAt { get; set; }
}
