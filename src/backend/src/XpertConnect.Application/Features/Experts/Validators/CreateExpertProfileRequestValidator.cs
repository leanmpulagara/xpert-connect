using FluentValidation;
using XpertConnect.Application.Features.Experts.DTOs;

namespace XpertConnect.Application.Features.Experts.Validators;

public class CreateExpertProfileRequestValidator : AbstractValidator<CreateExpertProfileRequest>
{
    public CreateExpertProfileRequestValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .WithMessage("Invalid expert category");

        RuleFor(x => x.Headline)
            .MaximumLength(200)
            .WithMessage("Headline must not exceed 200 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .WithMessage("Bio must not exceed 2000 characters");

        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Hourly rate must be non-negative");

        RuleFor(x => x.Currency)
            .NotEmpty()
            .MaximumLength(3)
            .WithMessage("Currency must be a valid 3-letter code");

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl))
            .WithMessage("LinkedIn URL must be a valid URL");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
