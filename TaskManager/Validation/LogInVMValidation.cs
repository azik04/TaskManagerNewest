using FluentValidation;
using TaskManager.ViewModels.Users;

namespace TaskManager.Validation
{
    public class LogInVMValidator : AbstractValidator<LogInVM>
    {
        public LogInVMValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Username is required.")
                .Length(1, 100).WithMessage("Username must be between 1 and 100 characters.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Length(8, 100).WithMessage("Password must be at least 8 characters long."); // Adjust minimum length as needed
        }
    }
}
