namespace XpertConnect.Domain.Enums;

/// <summary>
/// Types of geofence events
/// </summary>
public enum GeofenceEventType
{
    /// <summary>
    /// User entered the geofence
    /// </summary>
    Entry,

    /// <summary>
    /// User exited the geofence
    /// </summary>
    Exit,

    /// <summary>
    /// User is dwelling within the geofence
    /// </summary>
    Dwell
}
