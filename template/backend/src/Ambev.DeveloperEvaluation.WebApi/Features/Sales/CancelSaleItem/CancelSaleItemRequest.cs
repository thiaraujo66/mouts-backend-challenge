namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

/// <summary>
/// Request model for cancelling a single item within a sale
/// </summary>
public class CancelSaleItemRequest
{
    public Guid SaleId { get; set; }
    public Guid ItemId { get; set; }
}
