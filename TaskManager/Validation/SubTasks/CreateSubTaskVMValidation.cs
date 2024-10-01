using FluentValidation;
using TaskManager.ViewModels.SubTask;

namespace TaskManager.Validation.SubTasks;

public class CreateSubTaskVMValidation : AbstractValidator<CreateSubTaskVM>
{
    public CreateSubTaskVMValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Alt tapşırığın adı tələb olunur.")
            .Length(1, 100).WithMessage("Alt tapşırığın adı 1-dən 100 simvola qədər olmalıdır.");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Prioritet tələb olunur.");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("İstifadəçi tələb olunur.");

        RuleFor(x => x.TaskId)
            .NotEmpty().WithMessage("Tapşırıq tələb olunur.");
    }
}
