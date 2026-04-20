using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Expert availability time slots
/// </summary>
public class ExpertAvailability : BaseEntity
{
    public Guid ExpertId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsRecurring { get; set; } = true;
    public DateTime? SpecificDate { get; set; }

    // Navigation properties
    public virtual Expert Expert { get; set; } = null!;
}
