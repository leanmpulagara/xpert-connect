namespace XpertConnect.Application.Features.Experts.DTOs;

public class AvailabilityResponse
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public string DayOfWeekName => DayOfWeek.ToString();
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsRecurring { get; set; }
    public DateTime? SpecificDate { get; set; }
}
