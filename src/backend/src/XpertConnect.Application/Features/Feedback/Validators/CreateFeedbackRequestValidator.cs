using FluentValidation;
using XpertConnect.Application.Features.Feedback.DTOs;

namespace XpertConnect.Application.Features.Feedback.Validators;

public class CreateFeedbackRequestValidator : AbstractValidator<CreateFeedbackRequest>
{
    public CreateFeedbackRequestValidator()
    {
        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5)
            .WithMessage("Rating must be between 1 and 5");

        RuleFor(x => x.Comments)
            .MaximumLength(2000)
            .WithMessage("Comments must not exceed 2000 characters");
    }
}
