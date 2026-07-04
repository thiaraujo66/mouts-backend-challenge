using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

/// <summary>
/// Request model for listing sales, following the API's general pagination/filtering
/// conventions (see .doc/general-api.md): _page, _size, _order and _min/_max range filters.
/// </summary>
public class GetSalesRequest
{
    [FromQuery(Name = "_page")]
    public int Page { get; set; } = 1;

    [FromQuery(Name = "_size")]
    public int Size { get; set; } = 10;

    [FromQuery(Name = "_order")]
    public string? Order { get; set; }

    [FromQuery(Name = "customerId")]
    public Guid? CustomerId { get; set; }

    [FromQuery(Name = "branchId")]
    public Guid? BranchId { get; set; }

    [FromQuery(Name = "isCancelled")]
    public bool? IsCancelled { get; set; }

    [FromQuery(Name = "saleNumber")]
    public string? SaleNumber { get; set; }

    [FromQuery(Name = "_minDate")]
    public DateTime? MinDate { get; set; }

    [FromQuery(Name = "_maxDate")]
    public DateTime? MaxDate { get; set; }

    [FromQuery(Name = "_minTotalAmount")]
    public decimal? MinTotalAmount { get; set; }

    [FromQuery(Name = "_maxTotalAmount")]
    public decimal? MaxTotalAmount { get; set; }
}
