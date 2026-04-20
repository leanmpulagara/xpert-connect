using FluentValidation;
using XpertConnect.Application.Features.Bids.DTOs;

namespace XpertConnect.Application.Features.Auctions.Validators;

public class PlaceBidRequestValidator : AbstractValidator<PlaceBidRequest>
{
    public PlaceBidRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Bid amount must be greater than 0");

        RuleFor(x => x.MaxProxyAmount)
            .GreaterThan(x => x.Amount)
            .When(x => x.IsProxyBid && x.MaxProxyAmount.HasValue)
            .WithMessage("Max proxy amount must be greater than bid amount");

        RuleFor(x => x.MaxProxyAmount)
            .NotNull()
            .When(x => x.IsProxyBid)
            .WithMessage("Max proxy amount is required for proxy bids");
    }
}
