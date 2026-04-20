using FluentValidation;
using XpertConnect.Application.Features.Consultations.DTOs;

namespace XpertConnect.Application.Features.Consultations.Validators;

public class CreateConsultationRequestValidator : AbstractValidator<CreateConsultationRequest>
{
    public CreateConsultationRequestValidator()
    {
        RuleFor(x => x.ExpertId)
            .NotEmpty()
            .WithMessage("Expert ID is required");

        RuleFor(x => x.ScheduledAt)
            .GreaterThan(DateTime.UtcNow.AddHours(1))
            .WithMessage("Consultation must be scheduled at least 1 hour in advance");

        RuleFor(x => x.DurationMinutes)
            .InclusiveBetween(15, 480)
            .WithMessage("Duration must be between 15 minutes and 8 hours");

        RuleFor(x => x.MeetingType)
            .IsInEnum()
            .WithMessage("Invalid meeting type");

        RuleFor(x => x.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes must not exceed 1000 characters");
    }
}
