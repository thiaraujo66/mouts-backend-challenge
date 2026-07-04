namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a single product line within a <see cref="Sale"/>, including the
/// quantity-based discount applied to it.
/// </summary>
public class SaleItem
{
    public Guid Id { get; private set; }
    public Guid SaleId { get; private set; }
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; } = string.Empty;
    public decimal UnitPrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal Discount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    private SaleItem() { }

    public SaleItem(Guid productId, string productName, decimal unitPrice, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new DomainException("Product name is required");

        if (unitPrice < 0)
            throw new DomainException("Unit price cannot be negative");

        if (quantity <= 0)
            throw new DomainException("Quantity must be greater than zero");

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        Quantity = quantity;
        Discount = CalculateDiscount(quantity);
        TotalAmount = CalculateTotal(unitPrice, quantity, Discount);
    }

    /// <summary>
    /// Encodes the challenge's quantity-based discount tiers:
    /// below 4 items: no discount; 4-9 items: 10%; 10-20 items: 20%; above 20 items: not allowed.
    /// </summary>
    public static decimal CalculateDiscount(int quantity)
    {
        if (quantity > 20)
            throw new DomainException("It's not possible to sell above 20 identical items");

        if (quantity >= 10)
            return 0.20m;

        if (quantity >= 4)
            return 0.10m;

        return 0m;
    }

    private static decimal CalculateTotal(decimal unitPrice, int quantity, decimal discount)
        => Math.Round(unitPrice * quantity * (1 - discount), 2);

    public void Cancel()
    {
        if (IsCancelled)
            throw new DomainException("Item is already cancelled");

        IsCancelled = true;
    }
}
