using FluentAssertions;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void User_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var user = new User();

        // Assert
        user.Email.Should().BeEmpty();
        user.FirstName.Should().BeEmpty();
        user.LastName.Should().BeEmpty();
        user.VerificationStatus.Should().Be(VerificationStatus.Pending);
        user.IsActive.Should().BeTrue();
        user.KycVerifications.Should().BeEmpty();
        user.ComplianceChecks.Should().BeEmpty();
    }

    [Fact]
    public void User_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";
        var firstName = "John";
        var lastName = "Doe";

        // Act
        var user = new User
        {
            Id = userId,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            UserType = UserType.Seeker,
            Phone = "+1234567890",
            ProfilePhotoUrl = "https://example.com/photo.jpg"
        };

        // Assert
        user.Id.Should().Be(userId);
        user.Email.Should().Be(email);
        user.FirstName.Should().Be(firstName);
        user.LastName.Should().Be(lastName);
        user.UserType.Should().Be(UserType.Seeker);
        user.Phone.Should().Be("+1234567890");
        user.ProfilePhotoUrl.Should().Be("https://example.com/photo.jpg");
    }

    [Theory]
    [InlineData(UserType.Seeker)]
    [InlineData(UserType.Expert)]
    [InlineData(UserType.NonProfitOrg)]
    [InlineData(UserType.Admin)]
    public void User_ShouldAcceptAllUserTypes(UserType userType)
    {
        // Arrange & Act
        var user = new User { UserType = userType };

        // Assert
        user.UserType.Should().Be(userType);
    }

    [Fact]
    public void User_ShouldTrackLastLogin()
    {
        // Arrange
        var user = new User();
        var loginTime = DateTime.UtcNow;

        // Act
        user.LastLoginAt = loginTime;

        // Assert
        user.LastLoginAt.Should().Be(loginTime);
    }
}
