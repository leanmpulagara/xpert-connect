using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Venue for physical meetings
/// </summary>
public class Venue : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? SecurityRating { get; set; }
    public bool IsPreApproved { get; set; } = false;
    public string? Amenities { get; set; }

    // Navigation properties
    public virtual Geofence? Geofence { get; set; }
    public virtual ICollection<PhysicalMeeting> PhysicalMeetings { get; set; } = new List<PhysicalMeeting>();
}
