using FluentValidation;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Validation.Users;

public class ChangePasswordVMValidation : AbstractValidator<ChangePasswordVM>
{
    public ChangePasswordVMValidation()
    {
        RuleFor(x => x.OldPassword)
            .NotEmpty().WithMessage("Köhnə şifrə tələb olunur.")
            .Length(1, 100).WithMessage("Şifrə 1-dən 100 simvola qədər olmalıdır.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifrə tələb olunur.")
            .Length(1, 100).WithMessage("Şifrə 1-dən 100 simvola qədər olmalıdır.");
    }
}
