using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// Validator for CancelSaleRequest
/// </summary>
public class CancelSaleRequestValidator : AbstractValidator<CancelSaleRequest>
{
    public CancelSaleRequestValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Sale ID is required");
    }
}
