using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sales record (aggregate root). References to Customer, Branch and Product are
/// kept as External Identities: only the external Id plus a denormalized description are stored,
/// since those domains are not owned by this bounded context.
/// </summary>
public class Sale : BaseEntity
{
    private readonly List<SaleItem> _items = new();

    public string SaleNumber { get; private set; } = string.Empty;
    public DateTime SaleDate { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; } = string.Empty;
    public Guid BranchId { get; private set; }
    public string BranchName { get; private set; } = string.Empty;
    public decimal TotalAmount { get; private set; }
    public SaleStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public IReadOnlyCollection<SaleItem> Items => _items.AsReadOnly();

    private Sale() { }

    public Sale(DateTime saleDate, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Customer name is required");

        if (string.IsNullOrWhiteSpace(branchName))
            throw new DomainException("Branch name is required");

        Id = Guid.NewGuid();
        SaleNumber = GenerateSaleNumber();
        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        Status = SaleStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    private static string GenerateSaleNumber()
        => $"SALE-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";

    public SaleItem AddItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        EnsureNotCancelled();

        var item = new SaleItem(productId, productName, unitPrice, quantity);
        _items.Add(item);
        RecalculateTotal();
        return item;
    }

    /// <summary>
    /// Replaces the full set of items, recalculating discounts and totals from scratch. Used by
    /// the Update use case, which treats the item list as a full replacement (not a patch).
    /// </summary>
    public void ReplaceItems(IEnumerable<(Guid ProductId, string ProductName, decimal UnitPrice, int Quantity)> items)
    {
        EnsureNotCancelled();

        _items.Clear();
        foreach (var i in items)
            _items.Add(new SaleItem(i.ProductId, i.ProductName, i.UnitPrice, i.Quantity));

        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDetails(DateTime saleDate, Guid customerId, string customerName, Guid branchId, string branchName)
    {
        EnsureNotCancelled();

        if (string.IsNullOrWhiteSpace(customerName))
            throw new DomainException("Customer name is required");

        if (string.IsNullOrWhiteSpace(branchName))
            throw new DomainException("Branch name is required");

        SaleDate = saleDate;
        CustomerId = customerId;
        CustomerName = customerName;
        BranchId = branchId;
        BranchName = branchName;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Sale is already cancelled");

        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void CancelItem(Guid itemId)
    {
        EnsureNotCancelled();

        var item = _items.FirstOrDefault(i => i.Id == itemId)
            ?? throw new KeyNotFoundException($"Item with ID {itemId} not found in sale {Id}");

        item.Cancel();
        RecalculateTotal();
        UpdatedAt = DateTime.UtcNow;
    }

    private void RecalculateTotal()
        => TotalAmount = _items.Where(i => !i.IsCancelled).Sum(i => i.TotalAmount);

    private void EnsureNotCancelled()
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot modify a cancelled sale");
    }
}
