using FluentValidation;
using TaskManager.ViewModels.Themes;

namespace TaskManager.Validation.Themes;

public class CreateThemeVMValidator : AbstractValidator<GetThemeVM>
{
    public CreateThemeVMValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Mövzu adı tələb olunur.")
            .Length(1, 100).WithMessage("Mövzu adı 1-dən 100 simvola qədər olmalıdır.");
    }
}
