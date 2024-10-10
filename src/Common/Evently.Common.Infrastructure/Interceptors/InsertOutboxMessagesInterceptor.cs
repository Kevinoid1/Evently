using Evently.Common.Domain.Abstractions;
using Evently.Common.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Evently.Common.Infrastructure.Interceptors;

public sealed class InsertOutboxMessagesInterceptor : SaveChangesInterceptor
{
#pragma warning disable S125
    /*
     // called after saving changes to the database
    public override async ValueTask<int> SavedChangesAsync(SaveChangesCompletedEventData eventData, int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if(eventData.Context is not null) // save changes has completed
        {
            await PublishDomainEventsAsync(eventData.Context);
        }
        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
    */
#pragma warning restore S125

    // called before saving changes to the database
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        if (eventData.Context != null)
        {
            SaveDomainEventsAsOutboxMessages(eventData.Context);
        }
        return base.SavingChanges(eventData, result);
    }

    private static void SaveDomainEventsAsOutboxMessages(DbContext context)
    {
        var outboxMessages = context.ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyCollection<IDomainEvent> events = entity.DomainEvents;
                entity.ClearDomainEvents();
                return events;
            })
            .Select(domainEvent => OutboxMessage.Create(domainEvent))
            .ToList();
        
        context.Set<OutboxMessage>().AddRange(outboxMessages);

#pragma warning disable S125
        /*var domainEvents = context.ChangeTracker
            .Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyCollection<IDomainEvent> events = entity.DomainEvents;
                entity.ClearDomainEvents();
                return events;
            })
            .ToList();*/
        
        /*using IServiceScope scope = serviceScopeFactory.CreateScope();
        IPublisher publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();
        foreach (IDomainEvent domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }*/
#pragma warning restore S125
    }
}
