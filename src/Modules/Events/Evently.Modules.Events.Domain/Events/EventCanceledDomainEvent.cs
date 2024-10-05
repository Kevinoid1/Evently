using Evently.Common.Domain.Abstractions;

namespace Evently.Modules.Events.Domain.Events;

public sealed class EventCanceledDomainEvent(Guid eventId): DomainEvent
{
    public Guid EventId { get; } = eventId;
}
