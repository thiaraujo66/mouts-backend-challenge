using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common;

/// <summary>
/// Shared AutoMapper profile mapping between Sales request/response DTOs and the
/// application-layer input/result DTOs. Reused by every Sales feature.
/// </summary>
public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        CreateMap<SaleItemRequest, SaleItemInput>();
        CreateMap<SaleItemResult, SaleItemResponse>();
        CreateMap<SaleResult, SaleResponse>();
    }
}
