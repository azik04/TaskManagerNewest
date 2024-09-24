using FluentValidation;
using TaskManager.ViewModels.Files;

namespace TaskManager.Validation;

public class CreateFileVMValidator : AbstractValidator<UploadFileVM>
{
    public CreateFileVMValidator()
    {
        RuleFor(x => x.File)
            .NotEmpty().WithMessage("File is required.")
            .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
            .Must(file => file.Length <= 25 * 1024 * 1024) 
            .WithMessage("File size must not exceed 25 MB.");
    }
}
