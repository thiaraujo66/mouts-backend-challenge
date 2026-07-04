namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// A single item line supplied by the caller when creating or updating a sale.
/// </summary>
public class SaleItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
