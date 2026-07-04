using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Shared AutoMapper profile for mapping Sale/SaleItem entities to application-layer results.
/// Reused by every Sales use case that returns a <see cref="SaleResult"/>.
/// </summary>
public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        CreateMap<Sale, SaleResult>();
        CreateMap<SaleItem, SaleItemResult>();
    }
}
