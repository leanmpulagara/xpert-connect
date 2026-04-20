using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using XpertConnect.Application.Common.Interfaces;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Services;

namespace XpertConnect.Application.Tests.Services;

public class GeofenceServiceTests
{
    private readonly Mock<ILogger<MockGeofenceService>> _loggerMock;
    private readonly MockGeofenceService _mockService;

    public GeofenceServiceTests()
    {
        _loggerMock = new Mock<ILogger<MockGeofenceService>>();
        _mockService = new MockGeofenceService(_loggerMock.Object);
    }

    [Fact]
    public async Task MockGeofenceService_IsWithinGeofence_ShouldReturnTrue()
    {
        // Arrange
        var geofenceId = Guid.NewGuid();
        var latitude = 40.7128;
        var longitude = -74.0060;

        // Act
        var result = await _mockService.IsWithinGeofenceAsync(geofenceId, latitude, longitude);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task MockGeofenceService_RecordGeofenceEvent_ShouldReturnValidEvent()
    {
        // Arrange
        var geofenceId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var latitude = 40.7128;
        var longitude = -74.0060;

        // Act
        var result = await _mockService.RecordGeofenceEventAsync(
            geofenceId, userId, latitude, longitude, GeofenceEventType.Entry);

        // Assert
        result.Should().NotBeNull();
        result.GeofenceId.Should().Be(geofenceId);
        result.UserId.Should().Be(userId);
        result.Latitude.Should().Be(latitude);
        result.Longitude.Should().Be(longitude);
        result.EventType.Should().Be(GeofenceEventType.Entry);
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task MockGeofenceService_VerifyMeetingAttendance_ShouldReturnTrue()
    {
        // Arrange
        var consultationId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var latitude = 40.7128;
        var longitude = -74.0060;

        // Act
        var result = await _mockService.VerifyMeetingAttendanceAsync(
            consultationId, userId, latitude, longitude);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task MockGeofenceService_CreateGeofence_ShouldReturnValidGeofence()
    {
        // Arrange
        var venueId = Guid.NewGuid();
        var latitude = 40.7128;
        var longitude = -74.0060;
        var radiusMeters = 150.0;

        // Act
        var result = await _mockService.CreateGeofenceAsync(
            venueId, latitude, longitude, radiusMeters);

        // Assert
        result.Should().NotBeNull();
        result.VenueId.Should().Be(venueId);
        result.CenterLatitude.Should().Be(latitude);
        result.CenterLongitude.Should().Be(longitude);
        result.RadiusMeters.Should().Be(radiusMeters);
        result.IsActive.Should().BeTrue();
        result.Id.Should().NotBeEmpty();
    }
}

public class GeofenceDistanceCalculationTests
{
    // Testing the Haversine formula logic
    // Using known distances for verification

    [Theory]
    [InlineData(40.7128, -74.0060, 40.7128, -74.0060, 0)] // Same point
    [InlineData(40.7128, -74.0060, 40.7580, -73.9855, 5.5)] // NYC to Central Park (~5.5km)
    public void CalculateDistance_ShouldReturnApproximateDistance(
        double lat1, double lon1, double lat2, double lon2, double expectedKm)
    {
        // Arrange & Act
        var distance = CalculateDistanceKm(lat1, lon1, lat2, lon2);

        // Assert
        if (expectedKm == 0)
        {
            distance.Should().Be(0);
        }
        else
        {
            distance.Should().BeApproximately(expectedKm, 0.5); // Within 500m tolerance
        }
    }

    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadiusKm = 6371.0;

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
