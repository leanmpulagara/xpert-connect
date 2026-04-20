using XpertConnect.Domain.Entities.Common;

namespace XpertConnect.Domain.Entities;

/// <summary>
/// Geofence for meeting location verification
/// </summary>
public class Geofence : AuditableEntity
{
    public Guid VenueId { get; set; }
    public string BoundaryType { get; set; } = "Circular"; // Circular or Polygonal
    public double CenterLatitude { get; set; }
    public double CenterLongitude { get; set; }
    public double RadiusMeters { get; set; } = 100;
    public string? PolygonCoords { get; set; } // JSON for polygonal boundaries
    public int DwellDurationMinutes { get; set; } = 60;
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public virtual Venue Venue { get; set; } = null!;
    public virtual ICollection<GeofenceEvent> GeofenceEvents { get; set; } = new List<GeofenceEvent>();
}
