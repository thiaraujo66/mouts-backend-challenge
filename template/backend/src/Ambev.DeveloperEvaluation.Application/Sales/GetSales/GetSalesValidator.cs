using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Validator for GetSalesCommand
/// </summary>
public class GetSalesValidator : AbstractValidator<GetSalesCommand>
{
    public GetSalesValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);

        RuleFor(x => x.MaxDate)
            .GreaterThanOrEqualTo(x => x.MinDate)
            .When(x => x.MinDate.HasValue && x.MaxDate.HasValue)
            .WithMessage("MaxDate must be greater than or equal to MinDate");

        RuleFor(x => x.MaxTotalAmount)
            .GreaterThanOrEqualTo(x => x.MinTotalAmount)
            .When(x => x.MinTotalAmount.HasValue && x.MaxTotalAmount.HasValue)
            .WithMessage("MaxTotalAmount must be greater than or equal to MinTotalAmount");
    }
}
