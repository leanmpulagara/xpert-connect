using XpertConnect.Domain.Entities.Common;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Geofence entry/exit event for meeting verification
/// </summary>
public class GeofenceEvent : BaseEntity
{
    public Guid GeofenceId { get; set; }
    public Guid UserId { get; set; }
    public GeofenceEventType EventType { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public virtual Geofence Geofence { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
