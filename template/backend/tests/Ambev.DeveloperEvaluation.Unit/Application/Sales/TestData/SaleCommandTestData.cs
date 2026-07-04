using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

/// <summary>
/// Provides methods for generating Sales command test data using the Bogus library.
/// </summary>
public static class SaleCommandTestData
{
    private static readonly Faker Faker = new();

    public static SaleItemInput GenerateValidItem(int quantity = 5)
    {
        return new SaleItemInput
        {
            ProductId = Guid.NewGuid(),
            ProductName = Faker.Commerce.ProductName(),
            UnitPrice = decimal.Parse(Faker.Commerce.Price(1, 1000)),
            Quantity = quantity
        };
    }

    public static CreateSaleCommand GenerateValidCreateCommand()
    {
        return new CreateSaleCommand
        {
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = Faker.Person.FullName,
            BranchId = Guid.NewGuid(),
            BranchName = Faker.Company.CompanyName(),
            Items = [GenerateValidItem()]
        };
    }

    public static UpdateSaleCommand GenerateValidUpdateCommand(Guid saleId)
    {
        return new UpdateSaleCommand
        {
            Id = saleId,
            SaleDate = DateTime.UtcNow,
            CustomerId = Guid.NewGuid(),
            CustomerName = Faker.Person.FullName,
            BranchId = Guid.NewGuid(),
            BranchName = Faker.Company.CompanyName(),
            Items = [GenerateValidItem()]
        };
    }
}
