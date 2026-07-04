using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleItemHandler"/> class.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given existing item When cancelling Then recalculates total and publishes event")]
    public async Task Handle_ExistingItem_CancelsAndPublishesEvent()
    {
        // Given
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        var item = sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);
        var expectedResult = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(ci => ci.Arg<Sale>());
        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        // When
        var result = await _handler.Handle(new CancelSaleItemCommand(sale.Id, item.Id), CancellationToken.None);

        // Then
        result.Should().Be(expectedResult);
        item.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Should().Be(0m);
        await _mediator.Received(1).Publish(Arg.Any<ItemCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given unknown item id When cancelling Then throws KeyNotFoundException")]
    public async Task Handle_UnknownItem_ThrowsKeyNotFoundException()
    {
        // Given
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.AddItem(Guid.NewGuid(), "Product A", 10m, 1);

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(new CancelSaleItemCommand(sale.Id, Guid.NewGuid()), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
