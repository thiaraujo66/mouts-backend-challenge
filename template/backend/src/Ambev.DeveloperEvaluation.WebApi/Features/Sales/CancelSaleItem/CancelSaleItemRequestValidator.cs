using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

/// <summary>
/// Validator for CancelSaleItemRequest
/// </summary>
public class CancelSaleItemRequestValidator : AbstractValidator<CancelSaleItemRequest>
{
    public CancelSaleItemRequestValidator()
    {
        RuleFor(x => x.SaleId).NotEmpty().WithMessage("Sale ID is required");
        RuleFor(x => x.ItemId).NotEmpty().WithMessage("Item ID is required");
    }
}
