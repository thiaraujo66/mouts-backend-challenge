using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale aggregate operations
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository
    /// </summary>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale (with its items) by its unique identifier
    /// </summary>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Persists changes made to a previously retrieved sale
    /// </summary>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a sale from the repository
    /// </summary>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a paged, filtered and ordered list of sales
    /// </summary>
    /// <param name="filter">Filter criteria</param>
    /// <param name="pageNumber">1-based page number</param>
    /// <param name="pageSize">Number of items per page</param>
    /// <param name="order">Ordering clause, e.g. "saleDate desc, totalAmount asc"</param>
    Task<(List<Sale> Items, int TotalCount)> GetPagedAsync(
        SaleFilter filter,
        int pageNumber,
        int pageSize,
        string? order,
        CancellationToken cancellationToken = default);
}
