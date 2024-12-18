﻿using Evently.Common.Application.EventBus;

namespace Evently.Modules.Events.IntegrationEvents;

public sealed class EventCanceledIntegrationEvent(Guid id, DateTime occurredOnUtc, Guid eventId)
    : IntegrationEvent(id, occurredOnUtc)
{
    public Guid EventId { get; init; } = eventId;
}
