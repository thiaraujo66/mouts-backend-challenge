using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Common;

/// <summary>
/// Domain-facing notifications for the optional Sale event log requested by the challenge.
/// No message broker is involved — <see cref="SaleEventLogger"/> just logs them.
/// </summary>
public record SaleCreatedEvent(Guid SaleId, string SaleNumber) : INotification;

public record SaleModifiedEvent(Guid SaleId, string SaleNumber) : INotification;

public record SaleCancelledEvent(Guid SaleId, string SaleNumber) : INotification;

public record ItemCancelledEvent(Guid SaleId, Guid ItemId) : INotification;
