using Ambev.DeveloperEvaluation.Application.Sales.Common;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Command for updating an existing sale. The item list is a full replacement, not a patch.
/// </summary>
public class UpdateSaleCommand : IRequest<SaleResult>
{
    public Guid Id { get; set; }
    public DateTime SaleDate { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;
    public List<SaleItemInput> Items { get; set; } = [];
}
