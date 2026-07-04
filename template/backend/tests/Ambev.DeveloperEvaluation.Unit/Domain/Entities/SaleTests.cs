using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the <see cref="Ambev.DeveloperEvaluation.Domain.Entities.Sale"/> and
/// <see cref="Ambev.DeveloperEvaluation.Domain.Entities.SaleItem"/> classes, focused on the
/// challenge's quantity-based discount rules and the cancellation invariants.
/// </summary>
public class SaleTests
{
    [Theory(DisplayName = "Quantity below 4 gets no discount")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Given_QuantityBelowFour_When_AddingItem_Then_NoDiscountApplied(int quantity)
    {
        var sale = SaleTestData.GenerateValidSale();
        var (productId, productName, unitPrice) = SaleTestData.GenerateValidItem();

        var item = sale.AddItem(productId, productName, unitPrice, quantity);

        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(unitPrice * quantity);
    }

    [Theory(DisplayName = "Quantity between 4 and 9 gets a 10% discount")]
    [InlineData(4)]
    [InlineData(9)]
    public void Given_QuantityBetweenFourAndNine_When_AddingItem_Then_TenPercentDiscountApplied(int quantity)
    {
        var sale = SaleTestData.GenerateValidSale();
        var (productId, productName, unitPrice) = SaleTestData.GenerateValidItem();

        var item = sale.AddItem(productId, productName, unitPrice, quantity);

        item.Discount.Should().Be(0.10m);
        item.TotalAmount.Should().Be(Math.Round(unitPrice * quantity * 0.90m, 2));
    }

    [Theory(DisplayName = "Quantity between 10 and 20 gets a 20% discount")]
    [InlineData(10)]
    [InlineData(20)]
    public void Given_QuantityBetweenTenAndTwenty_When_AddingItem_Then_TwentyPercentDiscountApplied(int quantity)
    {
        var sale = SaleTestData.GenerateValidSale();
        var (productId, productName, unitPrice) = SaleTestData.GenerateValidItem();

        var item = sale.AddItem(productId, productName, unitPrice, quantity);

        item.Discount.Should().Be(0.20m);
        item.TotalAmount.Should().Be(Math.Round(unitPrice * quantity * 0.80m, 2));
    }

    [Fact(DisplayName = "Quantity above 20 is rejected")]
    public void Given_QuantityAboveTwenty_When_AddingItem_Then_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        var (productId, productName, unitPrice) = SaleTestData.GenerateValidItem();

        var act = () => sale.AddItem(productId, productName, unitPrice, 21);

        act.Should().Throw<DomainException>().WithMessage("*above 20*");
    }

    [Fact(DisplayName = "Sale total is the sum of all item totals")]
    public void Given_MultipleItems_When_Added_Then_TotalAmountIsSumOfItems()
    {
        var sale = SaleTestData.GenerateValidSale();

        var item1 = sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);
        var item2 = sale.AddItem(Guid.NewGuid(), "Product B", 20m, 5);

        sale.TotalAmount.Should().Be(item1.TotalAmount + item2.TotalAmount);
    }

    [Fact(DisplayName = "Cancelling a sale sets its status to Cancelled")]
    public void Given_ActiveSale_When_Cancelled_Then_StatusIsCancelled()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);

        sale.Cancel();

        sale.Status.Should().Be(SaleStatus.Cancelled);
    }

    [Fact(DisplayName = "Cancelling an already cancelled sale throws")]
    public void Given_CancelledSale_When_CancelledAgain_Then_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var act = () => sale.Cancel();

        act.Should().Throw<DomainException>();
    }

    [Fact(DisplayName = "Cancelling an item recalculates the sale total excluding it")]
    public void Given_SaleWithItems_When_ItemCancelled_Then_TotalExcludesCancelledItem()
    {
        var sale = SaleTestData.GenerateValidSale();
        var item1 = sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);
        var item2 = sale.AddItem(Guid.NewGuid(), "Product B", 20m, 1);

        sale.CancelItem(item1.Id);

        item1.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(item2.TotalAmount);
    }

    [Fact(DisplayName = "Cancelling an unknown item throws")]
    public void Given_Sale_When_CancellingUnknownItem_Then_ThrowsKeyNotFoundException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);

        var act = () => sale.CancelItem(Guid.NewGuid());

        act.Should().Throw<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Modifying a cancelled sale throws")]
    public void Given_CancelledSale_When_AddingItem_Then_ThrowsDomainException()
    {
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var act = () => sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);

        act.Should().Throw<DomainException>();
    }
}
