using Evently.Common.Application.EventBus;
using Evently.Common.Application.Exceptions;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Evently.Modules.Events.Application.Events.GetEvent;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.IntegrationEvents;
using MediatR;

namespace Evently.Modules.Events.Application.Events.PublishEvent;

internal sealed class EventPublishedDomainEventHandler(ISender sender, IEventBus eventBus) : IDomainEventHandler<EventPublishedDomainEvent>
{
    public async Task Handle(EventPublishedDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        Result<EventResponse> eventResult = await sender.Send(new GetEventQuery(domainEvent.EventId), cancellationToken);

        if (eventResult.IsFailure)
        {
            throw new EventlyException(nameof(GetEventQuery), eventResult.Error);
        }

        EventResponse @event = eventResult.Value;
        await eventBus.PublishAsync(
            new EventPublishedIntegrationEvent(
                domainEvent.Id,
                domainEvent.OccurredOnUtc,
                domainEvent.EventId,
                @event.Title,
                @event.Description,
                @event.Location,
                @event.StartsAtUtc,
                @event.EndsAtUtc,
                @event.TicketTypes.Select(
                    ticketType => new TicketTypeModel
                    {
                        Id = ticketType.TicketTypeId,
                        EventId = @event.Id,
                        Name = ticketType.Name,
                        Price = ticketType.Price,
                        Currency = ticketType.Currency,
                        Quantity = ticketType.Quantity
                    }).ToList()), 
                cancellationToken);
    }
}
