using System.Reflection;
using Evently.Common.Application.EventBus;
using Evently.Common.Domain.Abstractions;

namespace Evently.Common.Application.Messaging;

public interface IEventPublisher
{
    Task PublishDomainEventAsync(IDomainEvent domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default);
    Task PublishDomainEventAsync(object domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default);
    
    Task PublishIntegrationEventAsync(IIntegrationEvent integrationEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default);
    Task PublishIntegrationEventAsync(object integrationEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default);
}
