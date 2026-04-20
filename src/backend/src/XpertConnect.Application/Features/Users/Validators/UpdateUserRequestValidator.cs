using FluentValidation;
using XpertConnect.Application.Features.Users.DTOs;

namespace XpertConnect.Application.Features.Users.Validators;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .When(x => x.FirstName != null);

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .When(x => x.LastName != null);

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .Matches(@"^\+?[\d\s\-\(\)]+$")
            .WithMessage("Phone number format is invalid")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.ProfilePhotoUrl)
            .MaximumLength(500)
            .Must(BeAValidUrl)
            .WithMessage("Profile photo URL must be a valid URL")
            .When(x => !string.IsNullOrEmpty(x.ProfilePhotoUrl));
    }

    private static bool BeAValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
