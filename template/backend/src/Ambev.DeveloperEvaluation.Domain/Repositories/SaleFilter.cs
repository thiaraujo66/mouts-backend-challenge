namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Filter criteria for listing sales, following the API's general filtering conventions
/// (exact match fields plus _min/_max range fields).
/// </summary>
public class SaleFilter
{
    public Guid? CustomerId { get; set; }
    public Guid? BranchId { get; set; }
    public bool? IsCancelled { get; set; }
    public string? SaleNumber { get; set; }
    public DateTime? MinDate { get; set; }
    public DateTime? MaxDate { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
}
