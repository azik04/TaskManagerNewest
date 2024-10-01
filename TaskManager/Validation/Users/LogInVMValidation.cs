using FluentValidation;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Validation.Users;

public class LogInVMValidator : AbstractValidator<LogInVM>
{
    public LogInVMValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("İstifadəçi adı tələb olunur.")
            .Length(1, 100).WithMessage("İstifadəçi adı 1-dən 100 simvola qədər olmalıdır.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifrə tələb olunur.")
            .Length(8, 100).WithMessage("Şifrə minimum 8 simvol uzunluğunda olmalıdır.");
    }
}
