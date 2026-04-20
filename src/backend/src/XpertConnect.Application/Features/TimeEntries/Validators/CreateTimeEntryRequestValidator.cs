using FluentValidation;
using XpertConnect.Application.Features.TimeEntries.DTOs;

namespace XpertConnect.Application.Features.TimeEntries.Validators;

public class CreateTimeEntryRequestValidator : AbstractValidator<CreateTimeEntryRequest>
{
    public CreateTimeEntryRequestValidator()
    {
        RuleFor(x => x.Hours)
            .GreaterThan(0).WithMessage("Hours must be greater than 0")
            .LessThanOrEqualTo(24).WithMessage("Hours cannot exceed 24 per entry");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters");

        RuleFor(x => x.LoggedAt)
            .LessThanOrEqualTo(DateTime.UtcNow.AddDays(1))
            .When(x => x.LoggedAt.HasValue)
            .WithMessage("Logged date cannot be in the future");
    }
}
