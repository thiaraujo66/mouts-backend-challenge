namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// API response model for a single sale item line.
/// </summary>
public class SaleItemResponse
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
}
