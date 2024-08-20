using Evently.Common.Domain.Abstractions;

namespace Evently.Modules.Events.Domain.Events;

public class EventCanceledDomainEvent(Guid eventId): DomainEvent
{
    public Guid EventId { get; } = eventId;
}
