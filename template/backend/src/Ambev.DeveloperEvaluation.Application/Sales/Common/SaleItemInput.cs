namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Represents a single item line supplied by the caller when creating or updating a sale.
/// Product is referenced as an External Identity (Id + denormalized name); unit price is
/// supplied by the caller since Product is not owned by this bounded context.
/// </summary>
public class SaleItemInput
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
