using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given active sale When cancelling Then updates status and publishes event")]
    public async Task Handle_ActiveSale_CancelsAndPublishesEvent()
    {
        // Given
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        var expectedResult = new SaleResult { Id = sale.Id, Status = SaleStatus.Cancelled };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(ci => ci.Arg<Sale>());
        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        // When
        var result = await _handler.Handle(new CancelSaleCommand(sale.Id), CancellationToken.None);

        // Then
        result.Status.Should().Be(SaleStatus.Cancelled);
        sale.Status.Should().Be(SaleStatus.Cancelled);
        await _mediator.Received(1).Publish(Arg.Any<SaleCancelledEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given already cancelled sale When cancelling again Then throws DomainException")]
    public async Task Handle_AlreadyCancelledSale_ThrowsDomainException()
    {
        // Given
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.Cancel();

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(new CancelSaleCommand(sale.Id), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact(DisplayName = "Given non-existing sale id When cancelling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var id = Guid.NewGuid();
        _saleRepository.GetByIdAsync(id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(new CancelSaleCommand(id), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
