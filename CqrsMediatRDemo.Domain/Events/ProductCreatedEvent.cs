using CqrsMediatRDemo.Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CqrsMediatRDemo.Domain.Events;

public record ProductCreatedEvent(
    Guid ProductId,
    string Name,
    string Description,
    Money Price,
    int InitialStock
) : DomainEvent, INotification;