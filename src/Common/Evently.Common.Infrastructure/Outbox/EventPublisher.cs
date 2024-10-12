using System.Collections.Concurrent;
using System.Reflection;
using Evently.Common.Application.EventBus;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Common.Infrastructure.Outbox;

public sealed class EventPublisher(IServiceProvider serviceProvider) : IEventPublisher
{
    // making it static makes the values to live in the class definition
    private static readonly ConcurrentDictionary<string, Type[]> _domainEventHandlersDictionary = new(); // outbox domain events
    private static readonly ConcurrentDictionary<string, Type[]> _integrationEventHandlersDictionary = new(); // inbox integration events
    public async Task PublishDomainEventAsync(IDomainEvent domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default) // need assembly because of the way the handlers are registered
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

    public Task PublishDomainEventAsync(object domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default)
    {
        return domainEvent switch
        {
            null => throw new ArgumentNullException(nameof(domainEvent)),
            IDomainEvent instance => PublishDomainEventAsync(instance, assemblyToScan, cancellationToken),
            _ => throw new ArgumentException($"{nameof(domainEvent)} does not implement ${nameof(IDomainEvent)}")
        };
    }

    public async Task PublishIntegrationEventAsync(IIntegrationEvent integrationEvent, Assembly assemblyToScan,
        CancellationToken cancellationToken = default)
    {
        List<IIntegrationEventHandler> handlers = [];
        foreach (Type integrationEventHandlerType in GetIntegrationEventHandlerTypes(assemblyToScan, integrationEvent.GetType()))
        {
            var integrationEventHandler = (IIntegrationEventHandler)serviceProvider.GetRequiredService(integrationEventHandlerType);
            handlers.Add(integrationEventHandler);
        }

        foreach (IIntegrationEventHandler integrationEventHandler in handlers)
        {
            await integrationEventHandler.Handle(integrationEvent, cancellationToken).ConfigureAwait(false);
        }
    }

    public Task PublishIntegrationEventAsync(object integrationEvent, Assembly assemblyToScan,
        CancellationToken cancellationToken = default)
    {
        return integrationEvent switch
        {
            null => throw new ArgumentNullException(nameof(integrationEvent)),
            IIntegrationEvent instance => PublishIntegrationEventAsync(instance, assemblyToScan, cancellationToken),
            _ => throw new ArgumentException(
                $"{nameof(integrationEvent)} does not implement ${nameof(IIntegrationEvent)}")
        };
    }

    private static Type[] GetDomainEventHandlerTypes(Assembly assemblyToScan, Type domainEventType) 
    {
        return _domainEventHandlersDictionary.GetOrAdd(
            $"{assemblyToScan.GetName().Name}{domainEventType.Name}",
            _ =>
            {
                Type[] domainEventHandlerTypes = assemblyToScan.GetTypes()
                    .Where(type => type.IsAssignableTo(typeof(IDomainEventHandler<>).MakeGenericType(domainEventType)))
                    .ToArray();

                return domainEventHandlerTypes;
            });
    }
    
    private static Type[] GetIntegrationEventHandlerTypes(Assembly assemblyToScan, Type integrationEventType) 
    {
        return _integrationEventHandlersDictionary.GetOrAdd(
            $"{assemblyToScan.GetName().Name}{integrationEventType.Name}",
            _ =>
            {
                Type[] integrationEventHandlerTypes = assemblyToScan.GetTypes()
                    .Where(type => type.IsAssignableTo(typeof(IIntegrationEventHandler<>).MakeGenericType(integrationEventType)))
                    .ToArray();

                return integrationEventHandlerTypes;
            });
    }
}
