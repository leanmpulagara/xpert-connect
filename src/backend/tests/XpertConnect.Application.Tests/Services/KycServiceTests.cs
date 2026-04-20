using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using XpertConnect.Domain.Enums;
using XpertConnect.Infrastructure.Services;

namespace XpertConnect.Application.Tests.Services;

public class MockKycServiceTests
{
    private readonly Mock<ILogger<MockKycService>> _loggerMock;
    private readonly MockKycService _mockService;

    public MockKycServiceTests()
    {
        _loggerMock = new Mock<ILogger<MockKycService>>();
        _mockService = new MockKycService(_loggerMock.Object);
    }

    [Fact]
    public async Task InitiateVerification_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var documentType = "passport";

        // Act
        var result = await _mockService.InitiateVerificationAsync(userId, documentType);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(VerificationStatus.Pending);
        result.ProviderRef.Should().StartWith("mock_check_");
    }

    [Fact]
    public async Task CheckVerificationStatus_ShouldReturnVerified()
    {
        // Arrange
        var providerRef = "mock_check_123";

        // Act
        var result = await _mockService.CheckVerificationStatusAsync(providerRef);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(VerificationStatus.Verified);
        result.ProviderRef.Should().Be(providerRef);
        result.VerifiedAt.Should().NotBeNull();
    }

    [Fact]
    public async Task PerformLivenessCheck_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var imageData = new byte[] { 0x89, 0x50, 0x4E, 0x47 }; // PNG header bytes

        // Act
        var result = await _mockService.PerformLivenessCheckAsync(userId, imageData);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("passport")]
    [InlineData("drivers_license")]
    [InlineData("national_id")]
    public async Task InitiateVerification_ShouldAcceptDifferentDocumentTypes(string documentType)
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Act
        var result = await _mockService.InitiateVerificationAsync(userId, documentType);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
