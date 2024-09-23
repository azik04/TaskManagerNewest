using FluentValidation;
using TaskManager.ViewModels.Tasks;

namespace TaskManager.Validation
{
    public class CreateTaskVMValidator : AbstractValidator<CreateTaskVM>
    {
        public CreateTaskVMValidator()
        {
            RuleFor(x => x.TaskName)
                .NotEmpty().WithMessage("Task name is required.")
                .Length(1, 100).WithMessage("Task name must be between 1 and 100 characters.");

            RuleFor(x => x.TaskDescription)
                .NotEmpty().WithMessage("Task description is required.")
                .MaximumLength(500).WithMessage("Task description must not exceed 500 characters.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.");

            RuleFor(x => x.ThemeId)
                .GreaterThan(0).WithMessage("Theme ID must be a positive number.");

            RuleFor(x => x.DeadLine)
                .NotEmpty().WithMessage("Deadline is required.");
        }

        private bool BeAValidDate(string deadLine)
        {
            return DateTime.TryParse(deadLine, out _);
        }
    }
}
