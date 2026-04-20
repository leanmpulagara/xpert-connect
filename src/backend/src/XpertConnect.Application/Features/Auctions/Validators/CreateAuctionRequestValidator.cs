using FluentValidation;
using XpertConnect.Application.Features.Auctions.DTOs;

namespace XpertConnect.Application.Features.Auctions.Validators;

public class CreateAuctionRequestValidator : AbstractValidator<CreateAuctionRequest>
{
    public CreateAuctionRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Title is required and must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .WithMessage("Description must not exceed 5000 characters");

        RuleFor(x => x.MeetingType)
            .IsInEnum()
            .WithMessage("Invalid meeting type");

        RuleFor(x => x.GuestLimit)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(20)
            .WithMessage("Guest limit must be between 0 and 20");

        RuleFor(x => x.StartingBid)
            .GreaterThan(0)
            .WithMessage("Starting bid must be greater than 0");

        RuleFor(x => x.BuyNowPrice)
            .GreaterThan(x => x.StartingBid)
            .When(x => x.BuyNowPrice.HasValue)
            .WithMessage("Buy now price must be greater than starting bid");

        RuleFor(x => x.StartTime)
            .GreaterThan(DateTime.UtcNow.AddHours(1))
            .WithMessage("Auction must start at least 1 hour from now");

        RuleFor(x => x.EndTime)
            .GreaterThan(x => x.StartTime)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.EndTime)
            .Must((request, endTime) => (endTime - request.StartTime).TotalHours >= 1)
            .WithMessage("Auction must run for at least 1 hour");
    }
}
