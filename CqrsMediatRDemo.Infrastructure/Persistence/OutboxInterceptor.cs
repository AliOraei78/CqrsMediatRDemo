using CqrsMediatRDemo.Domain.Entities;
using CqrsMediatRDemo.Domain.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CqrsMediatRDemo.Infrastructure.Persistence;

public class OutboxInterceptor : SaveChangesInterceptor
{
    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

        var outboxMessages = new List<OutboxMessage>();

        // Extract all Aggregates that have DomainEvents
        var aggregates = eventData.Context.ChangeTracker
            .Entries<IHasDomainEvents>() 
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        foreach (var aggregate in aggregates)
        {
            foreach (var domainEvent in aggregate.DomainEvents)
            {
                var message = new OutboxMessage
                {
                    Type = domainEvent.GetType().FullName!,
                    Payload = JsonSerializer.Serialize(domainEvent, domainEvent.GetType()),
                    OccurredOn = domainEvent.OccurredOn
                };

                outboxMessages.Add(message);
            }

            aggregate.ClearDomainEvents(); // Clear after extraction
        }

        if (outboxMessages.Any())
        {
            eventData.Context.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
