using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Command for retrieving a paged, filtered and ordered list of sales
/// </summary>
public class GetSalesCommand : IRequest<GetSalesResult>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Order { get; set; }

    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }
    public bool? IsCancelled { get; set; }
    public string? SaleNumber { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
}
