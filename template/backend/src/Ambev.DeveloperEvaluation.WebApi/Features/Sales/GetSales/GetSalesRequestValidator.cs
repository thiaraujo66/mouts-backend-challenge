using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

/// <summary>
/// Validator for GetSalesRequest
/// </summary>
public class GetSalesRequestValidator : AbstractValidator<GetSalesRequest>
{
    public GetSalesRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThanOrEqualTo(1);
        RuleFor(x => x.Size).InclusiveBetween(1, 100);
    }
}
