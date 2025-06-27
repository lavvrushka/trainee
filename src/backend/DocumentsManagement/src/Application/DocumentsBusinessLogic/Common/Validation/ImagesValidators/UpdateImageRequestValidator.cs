
using FluentValidation;

namespace DocumentsBusinessLogic.Common.Validation.ImagesValidators;

public class UpdateImageRequestValidator : AbstractValidator<UpdateImageRequest>
{
    private const long MaxFileSize = 5 * 1024 * 1024;
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png" };

    public UpdateImageRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEqual(Guid.Empty)
            .WithMessage("A valid image ID must be provided.");

        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("An image file must be provided for update.");

        When(x => x.File != null, () =>
        {
            RuleFor(x => x.File.Length)
                .GreaterThan(0)
                .WithMessage("The image file must not be empty.")
                .LessThanOrEqualTo(MaxFileSize)
                .WithMessage($"The maximum allowed image size is {MaxFileSize / (1024 * 1024)} MB.");

            RuleFor(x => x.File.FileName)
                .Must(HasValidExtension)
                .WithMessage($"Only image files with the following extensions are allowed: {string.Join(", ", AllowedExtensions)}.");

            RuleFor(x => x.File.ContentType)
                .NotEmpty()
                .WithMessage("Could not determine the file's content type.")
                .Must(ct => ct.StartsWith("image/"))
                .WithMessage("The file's content type must be an image.");
        });
    }

    private bool HasValidExtension(string fileName)
    {
        var ext = Path.GetExtension(fileName)?.ToLowerInvariant();

        return ext != null && AllowedExtensions.Contains(ext);
    }
}