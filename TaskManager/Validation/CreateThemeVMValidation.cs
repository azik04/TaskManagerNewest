using FluentValidation;
using TaskManager.ViewModels.Themes;

namespace TaskManager.Validation;

public class CreateThemeVMValidator : AbstractValidator<GetThemeVM>
{
    public CreateThemeVMValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Theme name is required.")
            .Length(1, 100).WithMessage("Theme name must be between 1 and 100 characters.");
    }
}
