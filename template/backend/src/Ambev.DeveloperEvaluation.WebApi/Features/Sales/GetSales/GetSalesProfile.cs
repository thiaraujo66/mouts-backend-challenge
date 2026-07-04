using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSales;

/// <summary>
/// Profile for mapping GetSales feature requests to commands
/// </summary>
public class GetSalesProfile : Profile
{
    public GetSalesProfile()
    {
        CreateMap<GetSalesRequest, GetSalesCommand>()
            .ForMember(dest => dest.PageNumber, opt => opt.MapFrom(src => src.Page))
            .ForMember(dest => dest.PageSize, opt => opt.MapFrom(src => src.Size));
    }
}
