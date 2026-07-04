using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

/// <summary>
/// Integration tests for <see cref="SaleRepository"/> against a real (in-memory Sqlite) database,
/// verifying that the EF Core mapping actually round-trips sales, items and enum conversions —
/// a gap unit tests with a mocked repository cannot cover.
/// </summary>
public class SaleRepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly DbContextOptions<DefaultContext> _options;

    public SaleRepositoryTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _options = new DbContextOptionsBuilder<DefaultContext>()
            .UseSqlite(_connection)
            .Options;

        using var context = new DefaultContext(_options);
        context.Database.EnsureCreated();
    }

    public void Dispose() => _connection.Dispose();

    private DefaultContext CreateContext() => new(_options);

    [Fact(DisplayName = "CreateAsync persists a sale together with its items")]
    public async Task CreateAsync_PersistsSaleWithItems()
    {
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.AddItem(Guid.NewGuid(), "Product A", 10m, 5);

        using (var context = CreateContext())
        {
            await new SaleRepository(context).CreateAsync(sale);
        }

        using (var context = CreateContext())
        {
            var persisted = await new SaleRepository(context).GetByIdAsync(sale.Id);

            Assert.NotNull(persisted);
            Assert.Equal(sale.SaleNumber, persisted!.SaleNumber);
            Assert.Single(persisted.Items);
            Assert.Equal(0.10m, persisted.Items.First().Discount);
            Assert.Equal(sale.TotalAmount, persisted.TotalAmount);
        }
    }

    [Fact(DisplayName = "UpdateAsync replaces items and recalculates total")]
    public async Task UpdateAsync_ReplacesItemsAndRecalculatesTotal()
    {
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.AddItem(Guid.NewGuid(), "Old product", 10m, 1);

        using (var context = CreateContext())
        {
            await new SaleRepository(context).CreateAsync(sale);
        }

        using (var context = CreateContext())
        {
            var repository = new SaleRepository(context);
            var tracked = await repository.GetByIdAsync(sale.Id);
            tracked!.ReplaceItems([(Guid.NewGuid(), "New product", 20m, 5)]);
            await repository.UpdateAsync(tracked);
        }

        using (var context = CreateContext())
        {
            var persisted = await new SaleRepository(context).GetByIdAsync(sale.Id);

            Assert.Single(persisted!.Items);
            Assert.Equal("New product", persisted.Items.First().ProductName);
            Assert.Equal(persisted.Items.First().TotalAmount, persisted.TotalAmount);
        }
    }

    [Fact(DisplayName = "DeleteAsync removes the sale and cascades to its items")]
    public async Task DeleteAsync_RemovesSaleAndItems()
    {
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);

        using (var context = CreateContext())
        {
            await new SaleRepository(context).CreateAsync(sale);
        }

        bool deleted;
        using (var context = CreateContext())
        {
            deleted = await new SaleRepository(context).DeleteAsync(sale.Id);
        }

        Assert.True(deleted);

        using (var context = CreateContext())
        {
            var persisted = await new SaleRepository(context).GetByIdAsync(sale.Id);
            Assert.Null(persisted);
            Assert.Empty(await context.SaleItems.Where(i => i.SaleId == sale.Id).ToListAsync());
        }
    }

    [Fact(DisplayName = "GetPagedAsync filters, sorts and paginates results")]
    public async Task GetPagedAsync_FiltersSortsAndPaginates()
    {
        var branchId = Guid.NewGuid();

        var saleA = new Sale(new DateTime(2026, 1, 1), Guid.NewGuid(), "Customer A", branchId, "Branch");
        saleA.AddItem(Guid.NewGuid(), "Product", 100m, 1);

        var saleB = new Sale(new DateTime(2026, 2, 1), Guid.NewGuid(), "Customer B", branchId, "Branch");
        saleB.AddItem(Guid.NewGuid(), "Product", 50m, 1);

        var saleOtherBranch = new Sale(new DateTime(2026, 3, 1), Guid.NewGuid(), "Customer C", Guid.NewGuid(), "Other Branch");
        saleOtherBranch.AddItem(Guid.NewGuid(), "Product", 10m, 1);

        using (var context = CreateContext())
        {
            var repository = new SaleRepository(context);
            await repository.CreateAsync(saleA);
            await repository.CreateAsync(saleB);
            await repository.CreateAsync(saleOtherBranch);
        }

        using (var context = CreateContext())
        {
            var repository = new SaleRepository(context);
            // Ordered by saleDate (not totalAmount) because the Sqlite provider used for this
            // test cannot translate ORDER BY over decimal columns; Postgres has no such limit.
            var (items, totalCount) = await repository.GetPagedAsync(
                new SaleFilter { BranchId = branchId }, pageNumber: 1, pageSize: 10, order: "saleDate desc");

            Assert.Equal(2, totalCount);
            Assert.Equal(2, items.Count);
            Assert.Equal(saleB.Id, items[0].Id);
            Assert.Equal(saleA.Id, items[1].Id);
        }
    }
}
