using FluentValidation;

namespace RetailerDisplay.Application.Media;

public class UploadUrlRequestValidator : AbstractValidator<UploadUrlRequest>
{
    public UploadUrlRequestValidator()
    {
        RuleFor(x => x.FileName).NotEmpty().MaximumLength(255);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SizeBytes).GreaterThan(0);
        RuleFor(x => x.Kind).IsInEnum();
    }
}
