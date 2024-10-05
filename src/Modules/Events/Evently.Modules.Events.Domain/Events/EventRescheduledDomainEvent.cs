using Evently.Common.Domain.Abstractions;

namespace Evently.Modules.Events.Domain.Events;

public sealed class EventRescheduledDomainEvent(Guid id, DateTime startsAtUtc, DateTime? endsAtUtc) : DomainEvent
{
    public Guid EventId { get; } = id;
    public DateTime StartsAtUtc { get; } = startsAtUtc;
    public DateTime? EndsAtUtc { get; } = endsAtUtc;
}
