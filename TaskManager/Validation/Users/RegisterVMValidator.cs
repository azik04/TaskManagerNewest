using FluentValidation;
using TaskManager.ViewModels.RegisterVM;

namespace TaskManager.Validation.Users;

public class RegisterVMValidator : AbstractValidator<RegisterVM>
{
    public RegisterVMValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("İstifadəçi adı tələb olunur.")
            .Length(1, 100).WithMessage("İstifadəçi adı 1-dən 100 simvola qədər olmalıdır.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email tələb olunur.")
            .EmailAddress().WithMessage("Email formatı yanlışdır.")
            .Length(1, 100).WithMessage("Email 1-dən 100 simvola qədər olmalıdır.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifrə tələb olunur.")
            .Length(8, 100).WithMessage("Şifrə minimum 8 simvol uzunluğunda olmalıdır.");
    }
}
