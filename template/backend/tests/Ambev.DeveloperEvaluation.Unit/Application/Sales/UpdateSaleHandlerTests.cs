using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given existing sale When updating Then replaces items and publishes event")]
    public async Task Handle_ExistingSale_ReplacesItemsAndPublishesEvent()
    {
        // Given
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Old Customer", Guid.NewGuid(), "Old Branch");
        sale.AddItem(Guid.NewGuid(), "Old Product", 5m, 1);

        var command = SaleCommandTestData.GenerateValidUpdateCommand(sale.Id);
        var expectedResult = new SaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(ci => ci.Arg<Sale>());
        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().Be(expectedResult);
        sale.CustomerName.Should().Be(command.CustomerName);
        sale.Items.Should().ContainSingle(i => i.ProductName == command.Items[0].ProductName);
        await _mediator.Received(1).Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given cancelled sale When updating Then throws DomainException")]
    public async Task Handle_CancelledSale_ThrowsDomainException()
    {
        // Given
        var sale = new Sale(DateTime.UtcNow, Guid.NewGuid(), "Customer", Guid.NewGuid(), "Branch");
        sale.Cancel();

        var command = SaleCommandTestData.GenerateValidUpdateCommand(sale.Id);
        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact(DisplayName = "Given non-existing sale When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = SaleCommandTestData.GenerateValidUpdateCommand(Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given item quantity above 20 When updating sale Then throws validation exception")]
    public async Task Handle_QuantityAboveLimit_ThrowsValidationException()
    {
        // Given
        var command = SaleCommandTestData.GenerateValidUpdateCommand(Guid.NewGuid());
        command.Items[0].Quantity = 25;

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}
