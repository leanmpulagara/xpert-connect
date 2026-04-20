using FluentValidation;
using XpertConnect.Application.Features.Experts.DTOs;

namespace XpertConnect.Application.Features.Experts.Validators;

public class UpdateExpertProfileRequestValidator : AbstractValidator<UpdateExpertProfileRequest>
{
    public UpdateExpertProfileRequestValidator()
    {
        RuleFor(x => x.Category)
            .IsInEnum()
            .When(x => x.Category.HasValue)
            .WithMessage("Invalid expert category");

        RuleFor(x => x.Headline)
            .MaximumLength(200)
            .When(x => x.Headline != null)
            .WithMessage("Headline must not exceed 200 characters");

        RuleFor(x => x.Bio)
            .MaximumLength(2000)
            .When(x => x.Bio != null)
            .WithMessage("Bio must not exceed 2000 characters");

        RuleFor(x => x.HourlyRate)
            .GreaterThanOrEqualTo(0)
            .When(x => x.HourlyRate.HasValue)
            .WithMessage("Hourly rate must be non-negative");

        RuleFor(x => x.Currency)
            .MaximumLength(3)
            .When(x => x.Currency != null)
            .WithMessage("Currency must be a valid 3-letter code");

        RuleFor(x => x.LinkedInUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.LinkedInUrl))
            .WithMessage("LinkedIn URL must be a valid URL");

        RuleFor(x => x.SecurityLevel)
            .MaximumLength(50)
            .When(x => x.SecurityLevel != null)
            .WithMessage("Security level must not exceed 50 characters");
    }

    private bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrWhiteSpace(url)) return true;
        return Uri.TryCreate(url, UriKind.Absolute, out var result)
            && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
    }
}
