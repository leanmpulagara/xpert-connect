using FluentAssertions;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Tests.Entities;

public class GeofenceTests
{
    [Fact]
    public void Geofence_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var geofence = new Geofence();

        // Assert
        geofence.BoundaryType.Should().Be("Circular");
        geofence.RadiusMeters.Should().Be(100);
        geofence.IsActive.Should().BeTrue();
        geofence.GeofenceEvents.Should().BeEmpty();
    }

    [Fact]
    public void Geofence_ShouldSetCoordinatesCorrectly()
    {
        // Arrange
        var venueId = Guid.NewGuid();
        var latitude = 40.7128;
        var longitude = -74.0060;
        var radius = 200.0;

        // Act
        var geofence = new Geofence
        {
            VenueId = venueId,
            CenterLatitude = latitude,
            CenterLongitude = longitude,
            RadiusMeters = radius
        };

        // Assert
        geofence.VenueId.Should().Be(venueId);
        geofence.CenterLatitude.Should().Be(latitude);
        geofence.CenterLongitude.Should().Be(longitude);
        geofence.RadiusMeters.Should().Be(radius);
    }

    [Fact]
    public void Geofence_ShouldSupportPolygonalBoundary()
    {
        // Arrange
        var polygonCoords = "[{\"lat\":40.7128,\"lng\":-74.0060},{\"lat\":40.7130,\"lng\":-74.0058}]";

        // Act
        var geofence = new Geofence
        {
            BoundaryType = "Polygonal",
            PolygonCoords = polygonCoords
        };

        // Assert
        geofence.BoundaryType.Should().Be("Polygonal");
        geofence.PolygonCoords.Should().Be(polygonCoords);
    }

    [Fact]
    public void Geofence_ShouldSetDwellDuration()
    {
        // Arrange & Act
        var geofence = new Geofence
        {
            DwellDurationMinutes = 90
        };

        // Assert
        geofence.DwellDurationMinutes.Should().Be(90);
    }
}

public class GeofenceEventTests
{
    [Fact]
    public void GeofenceEvent_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var geofenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var timestamp = DateTime.UtcNow;

        // Act
        var geofenceEvent = new GeofenceEvent
        {
            Id = eventId,
            GeofenceId = geofenceId,
            UserId = userId,
            EventType = GeofenceEventType.Entry,
            Latitude = 40.7128,
            Longitude = -74.0060,
            Timestamp = timestamp
        };

        // Assert
        geofenceEvent.Id.Should().Be(eventId);
        geofenceEvent.GeofenceId.Should().Be(geofenceId);
        geofenceEvent.UserId.Should().Be(userId);
        geofenceEvent.EventType.Should().Be(GeofenceEventType.Entry);
        geofenceEvent.Latitude.Should().Be(40.7128);
        geofenceEvent.Longitude.Should().Be(-74.0060);
        geofenceEvent.Timestamp.Should().Be(timestamp);
    }

    [Theory]
    [InlineData(GeofenceEventType.Entry)]
    [InlineData(GeofenceEventType.Exit)]
    [InlineData(GeofenceEventType.Dwell)]
    public void GeofenceEvent_ShouldAcceptAllEventTypes(GeofenceEventType eventType)
    {
        // Arrange & Act
        var geofenceEvent = new GeofenceEvent { EventType = eventType };

        // Assert
        geofenceEvent.EventType.Should().Be(eventType);
    }
}
