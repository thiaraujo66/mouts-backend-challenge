using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(List<Sale> Items, int TotalCount)> GetPagedAsync(
        SaleFilter filter,
        int pageNumber,
        int pageSize,
        string? order,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.Include(s => s.Items).AsQueryable();

        if (filter.CustomerId.HasValue)
            query = query.Where(s => s.CustomerId == filter.CustomerId.Value);

        if (filter.BranchId.HasValue)
            query = query.Where(s => s.BranchId == filter.BranchId.Value);

        if (filter.IsCancelled.HasValue)
        {
            var status = filter.IsCancelled.Value ? Domain.Enums.SaleStatus.Cancelled : Domain.Enums.SaleStatus.Active;
            query = query.Where(s => s.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(filter.SaleNumber))
            query = query.Where(s => s.SaleNumber == filter.SaleNumber);

        if (filter.MinDate.HasValue)
            query = query.Where(s => s.SaleDate >= filter.MinDate.Value);

        if (filter.MaxDate.HasValue)
            query = query.Where(s => s.SaleDate <= filter.MaxDate.Value);

        if (filter.MinTotalAmount.HasValue)
            query = query.Where(s => s.TotalAmount >= filter.MinTotalAmount.Value);

        if (filter.MaxTotalAmount.HasValue)
            query = query.Where(s => s.TotalAmount <= filter.MaxTotalAmount.Value);

        query = ApplyOrder(query, order);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Sale> ApplyOrder(IQueryable<Sale> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderByDescending(s => s.SaleDate);

        IOrderedQueryable<Sale>? orderedQuery = null;

        foreach (var clause in order.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var parts = clause.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var keySelector = GetKeySelector(parts[0]);
            var descending = parts.Length > 1 && parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);

            orderedQuery = orderedQuery == null
                ? (descending ? query.OrderByDescending(keySelector) : query.OrderBy(keySelector))
                : (descending ? orderedQuery.ThenByDescending(keySelector) : orderedQuery.ThenBy(keySelector));
        }

        return orderedQuery ?? query.OrderByDescending(s => s.SaleDate);
    }

    private static System.Linq.Expressions.Expression<Func<Sale, object>> GetKeySelector(string field) => field.ToLowerInvariant() switch
    {
        "salenumber" => s => s.SaleNumber,
        "saledate" => s => s.SaleDate,
        "customername" => s => s.CustomerName,
        "branchname" => s => s.BranchName,
        "totalamount" => s => s.TotalAmount,
        "status" => s => s.Status,
        _ => s => s.SaleDate
    };
}
