using FluentValidation;
using TaskManager.ViewModels.Files;

namespace TaskManager.Validation.Files;

public class CreateFileVMValidator : AbstractValidator<UploadFileVM>
{
    public CreateFileVMValidator()
    {
        RuleFor(x => x.File)
            .NotEmpty().WithMessage("Fayl tələb olunur.")
            .Must(file => file.Length > 0).WithMessage("Fayl boş ola bilməz.")
            .Must(file => file.Length <= 25 * 1024 * 1024)
            .WithMessage("Faylın ölçüsü 25 MB-dan artıq olmamalıdır.");
    }
}
