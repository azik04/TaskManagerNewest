using FluentValidation;
using TaskManager.ViewModels.UserTask;

namespace TaskManager.Validation
{
    public class CreateUserTaskVMValidation : AbstractValidator<CreateUserTaskVM>
    {
        public CreateUserTaskVMValidation()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is required.");
        }
    }
}
