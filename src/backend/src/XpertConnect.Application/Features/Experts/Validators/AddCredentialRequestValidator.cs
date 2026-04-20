using FluentValidation;
using XpertConnect.Application.Features.Experts.DTOs;

namespace XpertConnect.Application.Features.Experts.Validators;

public class AddCredentialRequestValidator : AbstractValidator<AddCredentialRequest>
{
    public AddCredentialRequestValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Credential type is required and must not exceed 100 characters");

        RuleFor(x => x.IssuingBody)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Issuing body is required and must not exceed 200 characters");

        RuleFor(x => x.ExpiryDate)
            .GreaterThan(x => x.IssueDate)
            .When(x => x.IssueDate.HasValue && x.ExpiryDate.HasValue)
            .WithMessage("Expiry date must be after issue date");

        RuleFor(x => x.VerificationSource)
            .MaximumLength(500)
            .When(x => x.VerificationSource != null);
    }
}
