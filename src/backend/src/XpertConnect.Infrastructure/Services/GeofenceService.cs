using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Infrastructure.Services;

/// <summary>
/// Geofence service implementation for GPS-based meeting verification
/// </summary>
public class GeofenceService : IGeofenceService
{
    private readonly ILogger<GeofenceService> _logger;
    private readonly IApplicationDbContext _context;
    private const double EarthRadiusKm = 6371.0;

    public GeofenceService(IApplicationDbContext context, ILogger<GeofenceService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsWithinGeofenceAsync(
        Guid geofenceId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        var geofence = await _context.Geofences
            .FirstOrDefaultAsync(g => g.Id == geofenceId, cancellationToken);

        if (geofence == null)
        {
            _logger.LogWarning("Geofence {GeofenceId} not found", geofenceId);
            return false;
        }

        var distance = CalculateDistanceKm(
            geofence.CenterLatitude,
            geofence.CenterLongitude,
            latitude,
            longitude);

        var distanceMeters = distance * 1000;
        var isWithin = distanceMeters <= geofence.RadiusMeters;

        _logger.LogInformation(
            "Location ({Lat}, {Lng}) is {Distance}m from geofence center. Within: {IsWithin}",
            latitude, longitude, distanceMeters, isWithin);

        return isWithin;
    }

    public async Task<GeofenceEvent> RecordGeofenceEventAsync(
        Guid geofenceId,
        Guid userId,
        double latitude,
        double longitude,
        GeofenceEventType eventType,
        CancellationToken cancellationToken = default)
    {
        var geofenceEvent = new GeofenceEvent
        {
            Id = Guid.NewGuid(),
            GeofenceId = geofenceId,
            UserId = userId,
            Latitude = latitude,
            Longitude = longitude,
            EventType = eventType,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        _context.GeofenceEvents.Add(geofenceEvent);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Recorded geofence event {EventType} for user {UserId} at geofence {GeofenceId}",
            eventType, userId, geofenceId);

        return geofenceEvent;
    }

    public async Task<bool> VerifyMeetingAttendanceAsync(
        Guid consultationId,
        Guid userId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        // Find the consultation and its associated physical meeting
        var consultation = await _context.Consultations
            .Include(c => c.PhysicalMeeting)
                .ThenInclude(pm => pm != null ? pm.Geofence : null)
            .FirstOrDefaultAsync(c => c.Id == consultationId, cancellationToken);

        if (consultation?.PhysicalMeeting?.Geofence == null)
        {
            _logger.LogWarning("No geofence configured for consultation {ConsultationId}", consultationId);
            return false;
        }

        var geofence = consultation.PhysicalMeeting.Geofence;
        var isWithin = await IsWithinGeofenceAsync(geofence.Id, latitude, longitude, cancellationToken);

        if (isWithin)
        {
            // Record the attendance event
            await RecordGeofenceEventAsync(
                geofence.Id,
                userId,
                latitude,
                longitude,
                GeofenceEventType.Entry,
                cancellationToken);

            _logger.LogInformation(
                "Meeting attendance verified for user {UserId} at consultation {ConsultationId}",
                userId, consultationId);
        }

        return isWithin;
    }

    public async Task<Geofence> CreateGeofenceAsync(
        Guid venueId,
        double latitude,
        double longitude,
        double radiusMeters,
        CancellationToken cancellationToken = default)
    {
        var geofence = new Geofence
        {
            Id = Guid.NewGuid(),
            VenueId = venueId,
            CenterLatitude = latitude,
            CenterLongitude = longitude,
            RadiusMeters = radiusMeters,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Geofences.Add(geofence);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Created geofence {GeofenceId} at ({Lat}, {Lng}) with radius {Radius}m for venue {VenueId}",
            geofence.Id, latitude, longitude, radiusMeters, venueId);

        return geofence;
    }

    /// <summary>
    /// Calculate distance between two points using Haversine formula
    /// </summary>
    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return EarthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }
}

/// <summary>
/// Mock geofence service for development/testing
/// </summary>
public class MockGeofenceService : IGeofenceService
{
    private readonly ILogger<MockGeofenceService> _logger;

    public MockGeofenceService(ILogger<MockGeofenceService> logger)
    {
        _logger = logger;
    }

    public Task<bool> IsWithinGeofenceAsync(
        Guid geofenceId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK GEOFENCE] Checking location ({Lat}, {Lng}) against geofence {GeofenceId}",
            latitude, longitude, geofenceId);

        // Always return true for testing
        return Task.FromResult(true);
    }

    public Task<GeofenceEvent> RecordGeofenceEventAsync(
        Guid geofenceId,
        Guid userId,
        double latitude,
        double longitude,
        GeofenceEventType eventType,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK GEOFENCE] Recording {EventType} event for user {UserId}",
            eventType, userId);

        var evt = new GeofenceEvent
        {
            Id = Guid.NewGuid(),
            GeofenceId = geofenceId,
            UserId = userId,
            Latitude = latitude,
            Longitude = longitude,
            EventType = eventType,
            Timestamp = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        return Task.FromResult(evt);
    }

    public Task<bool> VerifyMeetingAttendanceAsync(
        Guid consultationId,
        Guid userId,
        double latitude,
        double longitude,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK GEOFENCE] Verifying attendance for user {UserId} at consultation {ConsultationId}",
            userId, consultationId);

        return Task.FromResult(true);
    }

    public Task<Geofence> CreateGeofenceAsync(
        Guid venueId,
        double latitude,
        double longitude,
        double radiusMeters,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("[MOCK GEOFENCE] Creating geofence for venue {VenueId}", venueId);

        var geofence = new Geofence
        {
            Id = Guid.NewGuid(),
            VenueId = venueId,
            CenterLatitude = latitude,
            CenterLongitude = longitude,
            RadiusMeters = radiusMeters,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        return Task.FromResult(geofence);
    }
}
