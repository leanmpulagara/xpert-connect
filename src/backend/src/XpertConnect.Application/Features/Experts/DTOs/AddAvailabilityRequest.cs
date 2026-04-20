namespace XpertConnect.Application.Features.Experts.DTOs;

/// <summary>
/// Request to add availability slot
/// </summary>
public class AddAvailabilityRequest
{
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsRecurring { get; set; } = true;
    public DateTime? SpecificDate { get; set; }
}
