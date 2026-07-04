using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// Profile for mapping CancelSale feature requests to commands
/// </summary>
public class CancelSaleProfile : Profile
{
    public CancelSaleProfile()
    {
        CreateMap<Guid, Application.Sales.CancelSale.CancelSaleCommand>()
            .ConstructUsing(id => new Application.Sales.CancelSale.CancelSaleCommand(id));
    }
}
