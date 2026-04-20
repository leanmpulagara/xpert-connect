using FluentAssertions;
using XpertConnect.Domain.Entities;
using XpertConnect.Domain.Enums;

namespace XpertConnect.Domain.Tests.Entities;

public class AuctionLotTests
{
    [Fact]
    public void AuctionLot_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var auction = new AuctionLot();

        // Assert
        auction.Title.Should().BeEmpty();
        auction.GuestLimit.Should().Be(0);
        auction.Status.Should().Be(AuctionStatus.Draft);
        auction.Bids.Should().BeEmpty();
    }

    [Fact]
    public void AuctionLot_ShouldSetPropertiesCorrectly()
    {
        // Arrange
        var auctionId = Guid.NewGuid();
        var expertId = Guid.NewGuid();
        var startTime = DateTime.UtcNow.AddDays(7);
        var endTime = DateTime.UtcNow.AddDays(14);

        // Act
        var auction = new AuctionLot
        {
            Id = auctionId,
            ExpertId = expertId,
            Title = "Lunch with Warren Buffett",
            Description = "A once-in-a-lifetime opportunity",
            MeetingType = MeetingType.InPerson_Lunch,
            GuestLimit = 7,
            StartingBid = 25000m,
            BuyNowPrice = 500000m,
            StartTime = startTime,
            EndTime = endTime,
            Status = AuctionStatus.Open
        };

        // Assert
        auction.Id.Should().Be(auctionId);
        auction.ExpertId.Should().Be(expertId);
        auction.Title.Should().Be("Lunch with Warren Buffett");
        auction.MeetingType.Should().Be(MeetingType.InPerson_Lunch);
        auction.GuestLimit.Should().Be(7);
        auction.StartingBid.Should().Be(25000m);
        auction.BuyNowPrice.Should().Be(500000m);
        auction.Status.Should().Be(AuctionStatus.Open);
    }

    [Theory]
    [InlineData(AuctionStatus.Draft)]
    [InlineData(AuctionStatus.Open)]
    [InlineData(AuctionStatus.Closed)]
    [InlineData(AuctionStatus.Cancelled)]
    public void AuctionLot_ShouldAcceptAllStatuses(AuctionStatus status)
    {
        // Arrange & Act
        var auction = new AuctionLot { Status = status };

        // Assert
        auction.Status.Should().Be(status);
    }

    [Fact]
    public void AuctionLot_ShouldTrackCurrentHighBid()
    {
        // Arrange
        var auction = new AuctionLot
        {
            StartingBid = 1000m,
            CurrentHighBid = null
        };

        // Act
        auction.CurrentHighBid = 1500m;

        // Assert
        auction.CurrentHighBid.Should().Be(1500m);
    }

    [Fact]
    public void AuctionLot_ShouldAllowBeneficiaryOrg()
    {
        // Arrange
        var beneficiaryId = Guid.NewGuid();

        // Act
        var auction = new AuctionLot
        {
            BeneficiaryOrgId = beneficiaryId
        };

        // Assert
        auction.BeneficiaryOrgId.Should().Be(beneficiaryId);
    }
}
