using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
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
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _mediator);
    }

    [Fact(DisplayName = "Given valid sale data When creating sale Then persists and returns result")]
    public async Task Handle_ValidRequest_ReturnsSaleResult()
    {
        // Given
        var command = SaleCommandTestData.GenerateValidCreateCommand();
        var expectedResult = new SaleResult { Id = Guid.NewGuid() };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Sale>());
        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(expectedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().Be(expectedResult);
        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
        await _mediator.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given item quantity above 20 When creating sale Then throws validation exception")]
    public async Task Handle_QuantityAboveLimit_ThrowsValidationException()
    {
        // Given
        var command = SaleCommandTestData.GenerateValidCreateCommand();
        command.Items[0].Quantity = 21;

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given no items When creating sale Then throws validation exception")]
    public async Task Handle_NoItems_ThrowsValidationException()
    {
        // Given
        var command = SaleCommandTestData.GenerateValidCreateCommand();
        command.Items.Clear();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact(DisplayName = "Given items with different quantities When creating sale Then applies correct discount tiers")]
    public async Task Handle_MultipleItems_AppliesDiscountTiers()
    {
        // Given
        var command = SaleCommandTestData.GenerateValidCreateCommand();
        command.Items =
        [
            new SaleItemInput { ProductId = Guid.NewGuid(), ProductName = "A", UnitPrice = 10m, Quantity = 2 },
            new SaleItemInput { ProductId = Guid.NewGuid(), ProductName = "B", UnitPrice = 10m, Quantity = 5 },
            new SaleItemInput { ProductId = Guid.NewGuid(), ProductName = "C", UnitPrice = 10m, Quantity = 15 }
        ];

        Sale? createdSale = null;
        _saleRepository.CreateAsync(Arg.Do<Sale>(s => createdSale = s), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Sale>());
        _mapper.Map<SaleResult>(Arg.Any<Sale>()).Returns(new SaleResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        createdSale.Should().NotBeNull();
        var items = createdSale!.Items.ToList();
        items[0].Discount.Should().Be(0m);
        items[1].Discount.Should().Be(0.10m);
        items[2].Discount.Should().Be(0.20m);
    }
}
