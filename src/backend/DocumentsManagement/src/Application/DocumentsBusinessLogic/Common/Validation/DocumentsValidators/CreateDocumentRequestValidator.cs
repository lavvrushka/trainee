using DocumentsBusinessLogic.UseCases.DocumentsUseCases;
using FluentValidation;

namespace DocumentsBusinessLogic.Common.Validation.DocumentsValidators;

public class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequest>
{
    private const long MaxFileSize = 10 * 1024 * 1024;

    public CreateDocumentRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("A file must be provided.");

        When(x => x.File != null, () =>
        {
            RuleFor(x => x.File.Length)
                .GreaterThan(0)
                .WithMessage("The file must not be empty.")
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage($"The maximum allowed file size is {MaxFileSize / (1024 * 1024)} MB.");

            RuleFor(x => x.File.ContentType)
                .NotEmpty()
                .WithMessage("Could not determine the file's content type.");
        });
    }
}