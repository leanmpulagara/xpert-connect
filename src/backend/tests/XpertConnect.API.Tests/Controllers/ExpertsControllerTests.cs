using System.Net;
using FluentAssertions;

namespace XpertConnect.API.Tests.Controllers;

public class ExpertsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ExpertsControllerTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetExperts_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/experts");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetExpertById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        var invalidId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/experts/{invalidId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetMyProfile_WithoutAuth_ShouldReturnUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/api/experts/me");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetExperts_WithCategoryFilter_ShouldReturnOk()
    {
        // Act
        var response = await _client.GetAsync("/api/experts?category=Technology");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
