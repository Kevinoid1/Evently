using System.Reflection;
using Evently.Common.Domain.Abstractions;

namespace Evently.Common.Application.Messaging;

public interface IDomainEventPublisher
{
    Task PublishAsync(IDomainEvent domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default);
    Task PublishAsync(object domainEvent, Assembly assemblyToScan, CancellationToken cancellationToken = default);
}
