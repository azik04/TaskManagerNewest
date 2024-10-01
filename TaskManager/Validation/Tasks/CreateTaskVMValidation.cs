using FluentValidation;
using TaskManager.ViewModels.Tasks;

namespace TaskManager.Validation.Tasks;

public class CreateTaskVMValidator : AbstractValidator<CreateTaskVM>
{
    public CreateTaskVMValidator()
    {
        RuleFor(x => x.TaskName)
            .NotEmpty().WithMessage("Tapşırıq adı tələb olunur.")
            .Length(1, 100).WithMessage("Tapşırıq adı 1-dən 100 simvola qədər olmalıdır.");

        RuleFor(x => x.TaskDescription)
            .NotEmpty().WithMessage("Tapşırıq təsviri tələb olunur.")
            .MaximumLength(500).WithMessage("Tapşırıq təsviri 500 simvoldan çox olmamalıdır.");

        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status tələb olunur.");

        RuleFor(x => x.Priority)
            .NotEmpty().WithMessage("Prioritet tələb olunur.");

        RuleFor(x => x.ThemeId)
            .GreaterThan(0).WithMessage("Mövzu ID-si müsbət bir ədəd olmalıdır.");

        RuleFor(x => x.DeadLine)
            .NotEmpty().WithMessage("Son tarix tələb olunur.");

        RuleFor(x => x.ExecutiveUserId)
            .NotEmpty().WithMessage("İcraçı istifadəçi tələb olunur.");
    }

    private bool BeAValidDate(string deadLine)
    {
        return DateTime.TryParse(deadLine, out _);
    }
}
