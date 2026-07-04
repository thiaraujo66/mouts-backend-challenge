using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.GetSales;

/// <summary>
/// Handler for processing GetSalesCommand requests
/// </summary>
public class GetSalesHandler : IRequestHandler<GetSalesCommand, GetSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public GetSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<GetSalesResult> Handle(GetSalesCommand request, CancellationToken cancellationToken)
    {
        var validator = new GetSalesValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var filter = new SaleFilter
        {
            CustomerId = request.CustomerId,
            BranchId = request.BranchId,
            IsCancelled = request.IsCancelled,
            SaleNumber = request.SaleNumber,
            MinDate = request.MinDate,
            MaxDate = request.MaxDate,
            MinTotalAmount = request.MinTotalAmount,
            MaxTotalAmount = request.MaxTotalAmount
        };

        var (items, totalCount) = await _saleRepository.GetPagedAsync(
            filter, request.PageNumber, request.PageSize, request.Order, cancellationToken);

        return new GetSalesResult
        {
            Items = _mapper.Map<IEnumerable<SaleResult>>(items),
            CurrentPage = request.PageNumber,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            TotalCount = totalCount
        };
    }
}
