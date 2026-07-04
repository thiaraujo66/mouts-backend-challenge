using Ambev.DeveloperEvaluation.Application.Sales.Common;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="GetSalesHandler"/> class.
/// </summary>
public class GetSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSalesHandler _handler;

    public GetSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given sales in repository When listing Then returns paged result")]
    public async Task Handle_ValidRequest_ReturnsPagedResult()
    {
        // Given
        var sales = new List<Sale>
        {
            new(DateTime.UtcNow, Guid.NewGuid(), "Customer A", Guid.NewGuid(), "Branch A"),
            new(DateTime.UtcNow, Guid.NewGuid(), "Customer B", Guid.NewGuid(), "Branch B")
        };
        var mappedResults = sales.Select(s => new SaleResult { Id = s.Id }).ToList();

        _saleRepository.GetPagedAsync(Arg.Any<SaleFilter>(), 1, 10, null, Arg.Any<CancellationToken>())
            .Returns((sales, 2));
        _mapper.Map<IEnumerable<SaleResult>>(sales).Returns(mappedResults);

        var command = new GetSalesCommand { PageNumber = 1, PageSize = 10 };

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Items.Should().BeEquivalentTo(mappedResults);
        result.TotalCount.Should().Be(2);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact(DisplayName = "Given invalid page size When listing Then throws validation exception")]
    public async Task Handle_InvalidPageSize_ThrowsValidationException()
    {
        // Given
        var command = new GetSalesCommand { PageNumber = 1, PageSize = 0 };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }
}
