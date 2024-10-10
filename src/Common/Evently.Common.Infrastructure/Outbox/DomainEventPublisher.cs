using System.Collections.Concurrent;
using System.Reflection;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Common.Infrastructure.Outbox;

public sealed class DomainEventPublisher(IServiceProvider serviceProvider) : IDomainEventPublisher
{
    // making it static makes the values to live in the class definition
    private static readonly ConcurrentDictionary<string, Type[]> _handlersDictionary = new();
    public async Task PublishAsync(IDomainEvent domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default) // need assembly because of the way the handlers are registered
    {
        List<IDomainEventHandler> handlers = [];
        foreach (Type domainEventHandlerType in GetDomainEventHandlerTypes(assemblyToScan, domainEvent.GetType()))
        {
            var domainEventHandler = (IDomainEventHandler)serviceProvider.GetRequiredService(domainEventHandlerType);
            handlers.Add(domainEventHandler);
        }

        foreach (IDomainEventHandler domainEventHandler in handlers)
        {
            await domainEventHandler.Handle(domainEvent, cancellationToken).ConfigureAwait(false);
        }
    }

    public Task PublishAsync(object domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default)
    {
        return domainEvent switch
        {
            null => throw new ArgumentNullException(nameof(domainEvent)),
            IDomainEvent instance => PublishAsync(instance, assemblyToScan, cancellationToken),
            _ => throw new ArgumentException($"{nameof(domainEvent)} does not implement ${nameof(IDomainEvent)}")
        };
    }
    
    private static Type[] GetDomainEventHandlerTypes(Assembly assemblyToScan, Type domainEventType) 
    {
        return _handlersDictionary.GetOrAdd(
            $"{assemblyToScan.GetName().Name}{domainEventType.Name}",
            _ =>
            {
                Type[] domainEventHandlerTypes = assemblyToScan.GetTypes()
                    .Where(type => type.IsAssignableTo(typeof(IDomainEventHandler<>).MakeGenericType(domainEventType)))
                    .ToArray();

                return domainEventHandlerTypes;
            });
    }
}
