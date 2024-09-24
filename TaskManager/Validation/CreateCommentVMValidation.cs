using FluentValidation;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Validation;

public class CreateCommentVMValidator : AbstractValidator<CreateCommentVM>
{
    public CreateCommentVMValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message is required.")
            .Length(1, 500).WithMessage("Message must be between 1 and 500 characters.");
    }
}
