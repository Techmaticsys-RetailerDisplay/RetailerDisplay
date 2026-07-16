using FluentValidation;

namespace RetailerDisplay.Application.Catalog;

public class CreateStoreProductRequestValidator : AbstractValidator<CreateStoreProductRequest>
{
    public CreateStoreProductRequestValidator()
    {
        RuleFor(x => x.Sku).NotEmpty().MaximumLength(80);
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0).When(x => x.SalePrice.HasValue);
        RuleFor(x => x.Currency).Length(3).When(x => !string.IsNullOrEmpty(x.Currency));
        RuleFor(x => x.Abv).InclusiveBetween(0, 100).When(x => x.Abv.HasValue);
    }
}

public class UpdateStoreProductRequestValidator : AbstractValidator<UpdateStoreProductRequest>
{
    public UpdateStoreProductRequestValidator()
    {
        RuleFor(x => x.ProductName).NotEmpty().MaximumLength(250);
        RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0).When(x => x.SalePrice.HasValue);
        RuleFor(x => x.Currency).Length(3).When(x => !string.IsNullOrEmpty(x.Currency));
        RuleFor(x => x.Abv).InclusiveBetween(0, 100).When(x => x.Abv.HasValue);
    }
}
