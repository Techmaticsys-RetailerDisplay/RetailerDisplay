using FluentValidation;

namespace RetailerDisplay.Application.Stores;

public class CreateStoreRequestValidator : AbstractValidator<CreateStoreRequest>
{
    public CreateStoreRequestValidator()
    {
        RuleFor(x => x.StoreName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.StoreCode).MaximumLength(50);
        RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(50);
    }
}

public class UpdateStoreRequestValidator : AbstractValidator<UpdateStoreRequest>
{
    public UpdateStoreRequestValidator()
    {
        RuleFor(x => x.StoreName).NotEmpty().MaximumLength(150);
        RuleFor(x => x.StoreCode).MaximumLength(50);
        RuleFor(x => x.TimeZone).NotEmpty().MaximumLength(50);
    }
}
