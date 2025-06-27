using FluentValidation;
using DocumentsBusinessLogic.UseCases.DocumentsUseCases;

namespace DocumentsBusinessLogic.Common.Validation.DocumentsValidators;

public class UpdateDocumentRequestValidator : AbstractValidator<UpdateDocumentRequest>
{
    private const long MaxFileSize = 10 * 1024 * 1024;

    public UpdateDocumentRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("A valid document ID must be provided.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("A file must be provided for update.");

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
