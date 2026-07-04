using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="DeleteSaleHandler"/> class.
/// </summary>
public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _handler = new DeleteSaleHandler(_saleRepository);
    }

    [Fact(DisplayName = "Given existing sale id When deleting sale Then returns success")]
    public async Task Handle_ExistingSale_ReturnsSuccess()
    {
        // Given
        var id = Guid.NewGuid();
        _saleRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(true);

        // When
        var result = await _handler.Handle(new DeleteSaleCommand(id), CancellationToken.None);

        // Then
        result.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "Given non-existing sale id When deleting sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var id = Guid.NewGuid();
        _saleRepository.DeleteAsync(id, Arg.Any<CancellationToken>()).Returns(false);

        // When
        var act = () => _handler.Handle(new DeleteSaleCommand(id), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
