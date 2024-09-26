using FluentValidation;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Validation;

public class ChangeEmailVMValidation : AbstractValidator<ChangeEmailVM>
{
    public ChangeEmailVMValidation()
    {
        RuleFor(x => x.OldEmail)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .Length(1, 100).WithMessage("Email must be between 1 and 100 characters.");

        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .Length(1, 100).WithMessage("Email must be between 1 and 100 characters.");

    }
}
