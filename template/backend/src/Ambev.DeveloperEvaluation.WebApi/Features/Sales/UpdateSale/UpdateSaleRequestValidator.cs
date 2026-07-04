using Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleRequest
/// </summary>
public class UpdateSaleRequestValidator : AbstractValidator<UpdateSaleRequest>
{
    public UpdateSaleRequestValidator()
    {
        RuleFor(sale => sale.CustomerId).NotEmpty();
        RuleFor(sale => sale.CustomerName).NotEmpty().MaximumLength(100);
        RuleFor(sale => sale.BranchId).NotEmpty();
        RuleFor(sale => sale.BranchName).NotEmpty().MaximumLength(100);
        RuleFor(sale => sale.Items).NotEmpty().WithMessage("A sale must have at least one item");
        RuleForEach(sale => sale.Items).SetValidator(new SaleItemRequestValidator());
    }
}
