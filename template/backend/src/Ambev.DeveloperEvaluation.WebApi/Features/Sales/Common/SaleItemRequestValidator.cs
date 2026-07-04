using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// Shared validation rules for sale item requests, used by both CreateSale and UpdateSale.
/// </summary>
public class SaleItemRequestValidator : AbstractValidator<SaleItemRequest>
{
    public SaleItemRequestValidator()
    {
        RuleFor(item => item.ProductId).NotEmpty();
        RuleFor(item => item.ProductName).NotEmpty().MaximumLength(100);
        RuleFor(item => item.UnitPrice).GreaterThan(0);
        RuleFor(item => item.Quantity)
            .InclusiveBetween(1, 20)
            .WithMessage("It's not possible to sell above 20 identical items");
    }
}
