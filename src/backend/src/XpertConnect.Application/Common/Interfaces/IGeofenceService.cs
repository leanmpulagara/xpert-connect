using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Application.Common.Interfaces;

/// <summary>
/// Geofence service interface for GPS-based meeting verification
/// </summary>
public interface IGeofenceService
{
    /// <summary>
    /// Check if a location is within a geofence
    /// </summary>
    Task<bool> IsWithinGeofenceAsync(Guid geofenceId, double latitude, double longitude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Record a geofence event (entry/exit)
    /// </summary>
    Task<GeofenceEvent> RecordGeofenceEventAsync(Guid geofenceId, Guid userId, double latitude, double longitude, GeofenceEventType eventType, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verify meeting attendance using geofence
    /// </summary>
    Task<bool> VerifyMeetingAttendanceAsync(Guid consultationId, Guid userId, double latitude, double longitude, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a geofence for a venue
    /// </summary>
    Task<Geofence> CreateGeofenceAsync(Guid venueId, double latitude, double longitude, double radiusMeters, CancellationToken cancellationToken = default);
}
