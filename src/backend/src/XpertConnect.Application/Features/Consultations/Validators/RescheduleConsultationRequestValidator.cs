using FluentValidation;
using XpertConnect.Application.Features.Consultations.DTOs;

namespace XpertConnect.Application.Features.Consultations.Validators;

public class RescheduleConsultationRequestValidator : AbstractValidator<RescheduleConsultationRequest>
{
    public RescheduleConsultationRequestValidator()
    {
        RuleFor(x => x.NewScheduledAt)
            .GreaterThan(DateTime.UtcNow.AddHours(1))
            .WithMessage("New schedule must be at least 1 hour in advance");

        RuleFor(x => x.Reason)
            .MaximumLength(500)
            .WithMessage("Reason must not exceed 500 characters");
    }
}
