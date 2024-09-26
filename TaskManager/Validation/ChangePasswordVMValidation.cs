using FluentValidation;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Validation;

public class ChangePasswordVMValidation : AbstractValidator<ChangePasswordVM>
{
    public ChangePasswordVMValidation()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Password is required.")
            .Length(1, 100).WithMessage("Password must be between 1 and 100 characters.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Password is required.")
            .Length(1, 100).WithMessage("Password must be between 1 and 100 characters.");

    }
}
