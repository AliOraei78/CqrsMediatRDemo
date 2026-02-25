using CqrsMediatRDemo.Domain.Events;

public interface IHasDomainEvents
{
    // Getting domain events
    IReadOnlyCollection<DomainEvent> DomainEvents { get; }
    // Clearing events after processing
    void ClearDomainEvents();
}