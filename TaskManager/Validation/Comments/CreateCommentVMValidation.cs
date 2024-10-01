using FluentValidation;
using TaskManager.ViewModels.Comments;

namespace TaskManager.Validation.Comments;

public class CreateCommentVMValidator : AbstractValidator<CreateCommentVM>
{
    public CreateCommentVMValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Mesaj tələb olunur.")
            .Length(1, 500).WithMessage("Mesaj 1-dən 500 simvola qədər olmalıdır.");
    }
}
