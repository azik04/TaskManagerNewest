using FluentValidation;
using TaskManager.ViewModels.SubTask;

namespace TaskManager.Validation
{
    public class CreateSubTaskVMValidation : AbstractValidator<CreateSubTaskVM>
    {
        public CreateSubTaskVMValidation()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("SubTask name is required.")
                .Length(1, 100).WithMessage("SubTask name must be between 1 and 100 characters.");

            RuleFor(x => x.Priority)
                .NotEmpty().WithMessage("Priority is required.");

            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("User is required.");

            RuleFor(x => x.TaskId)
             .NotEmpty().WithMessage("Task is required. ");


        }
    }
}
