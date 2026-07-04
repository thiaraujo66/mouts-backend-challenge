using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Logs Sale domain events to the application log. Stands in for the optional
/// SaleCreated/SaleModified/SaleCancelled/ItemCancelled event publication mentioned in the
/// challenge — no message broker is required, so we just log.
/// </summary>
public class SaleEventLogger :
    INotificationHandler<SaleCreatedEvent>,
    INotificationHandler<SaleModifiedEvent>,
    INotificationHandler<SaleCancelledEvent>,
    INotificationHandler<ItemCancelledEvent>
{
    private readonly ILogger<SaleEventLogger> _logger;

    public SaleEventLogger(ILogger<SaleEventLogger> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SaleCreated: sale {SaleId} ({SaleNumber}) was created", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SaleModified: sale {SaleId} ({SaleNumber}) was modified", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("SaleCancelled: sale {SaleId} ({SaleNumber}) was cancelled", notification.SaleId, notification.SaleNumber);
        return Task.CompletedTask;
    }

    public Task Handle(ItemCancelledEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("ItemCancelled: item {ItemId} of sale {SaleId} was cancelled", notification.ItemId, notification.SaleId);
        return Task.CompletedTask;
    }
}
