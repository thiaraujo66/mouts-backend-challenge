using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

/// <summary>
/// Profile for mapping CancelSaleItem feature requests to commands
/// </summary>
public class CancelSaleItemProfile : Profile
{
    public CancelSaleItemProfile()
    {
        CreateMap<CancelSaleItemRequest, CancelSaleItemCommand>();
    }
}
