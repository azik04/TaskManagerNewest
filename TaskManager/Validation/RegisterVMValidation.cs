using FluentValidation;
using TaskManager.ViewModels.UsersVMs;

namespace TaskManager.Validation;

public class RegisterVMValidator : AbstractValidator<AccountVM>
{
    public RegisterVMValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("Username is required.")
            .Length(1, 100).WithMessage("Username must be between 1 and 100 characters.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.")
            .Length(1, 100).WithMessage("Email must be between 1 and 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(8, 100).WithMessage("Password must be at least 8 characters long."); // Adjust minimum length as needed
    }
}
