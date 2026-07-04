using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing sale id When getting sale Then returns mapped result")]
    public async Task Handle_ExistingSale_ReturnsSaleResult()
    {
        // Given
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        var expectedResult = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<SaleResult>(sale).Returns(expectedResult);

        // When
        var result = await _handler.Handle(new GetSaleCommand(sale.Id), CancellationToken.None);

        // Then
        result.Should().Be(expectedResult);
    }

    [Fact(DisplayName = "Given non-existing sale id When getting sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var id = Guid.NewGuid();
        _saleRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(new GetSaleCommand(id), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
