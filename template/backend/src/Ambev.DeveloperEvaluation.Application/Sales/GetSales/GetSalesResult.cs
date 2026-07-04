using Ambev.DeveloperEvaluation.Application.Sales.Common;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Paged result for the GetSales use case
/// </summary>
public class GetSalesResult
{
    public IEnumerable<SaleResult> Items { get; set; } = [];
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}
