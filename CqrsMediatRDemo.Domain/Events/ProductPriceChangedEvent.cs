using CqrsMediatRDemo.Domain.Events;
using CqrsMediatRDemo.Domain.ValueObjects;
using MediatR;

namespace CqrsMediatRDemo.Domain.Events;

public record ProductPriceChangedEvent(
    Guid ProductId,
    Money OldPrice,
    Money NewPrice
) : DomainEvent, INotification;