using FluentValidation;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Validation.Users;

public class ChangeEmailVMValidation : AbstractValidator<ChangeEmailVM>
{
    public ChangeEmailVMValidation()
    {
        RuleFor(x => x.OldEmail)
            .NotEmpty().WithMessage("Köhnə email tələb olunur.")
            .EmailAddress().WithMessage("Email formatı yanlışdır.")
            .Length(1, 100).WithMessage("Email 1-dən 100 simvola qədər olmalıdır.");

        RuleFor(x => x.NewEmail)
            .NotEmpty().WithMessage("Yeni email tələb olunur.")
            .EmailAddress().WithMessage("Email formatı yanlışdır.")
            .Length(1, 100).WithMessage("Email 1-dən 100 simvola qədər olmalıdır.");
    }
}
