namespace CqrsMediatRDemo.Domain.Events;

public abstract record DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}