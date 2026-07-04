using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating Sale test data using the Bogus library.
/// </summary>
public static class SaleTestData
{
    private static readonly Faker Faker = new();

    public static Sale GenerateValidSale()
    {
        return new Sale(
            DateTime.UtcNow,
            Guid.NewGuid(),
            Faker.Company.CompanyName(),
            Guid.NewGuid(),
            Faker.Address.City());
    }

    public static (Guid ProductId, string ProductName, decimal UnitPrice) GenerateValidItem()
    {
        return (Guid.NewGuid(), Faker.Commerce.ProductName(), decimal.Parse(Faker.Commerce.Price(1, 1000)));
    }
}
