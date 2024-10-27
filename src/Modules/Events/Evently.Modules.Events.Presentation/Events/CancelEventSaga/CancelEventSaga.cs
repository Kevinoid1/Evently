using Evently.Common.Application.Clock;
using Evently.Modules.Events.IntegrationEvents;
using Evently.Modules.Ticketing.IntegrationEvents;
using MassTransit;

namespace Evently.Modules.Events.Presentation.Events.CancelEventSaga;

public sealed class CancelEventSaga : MassTransitStateMachine<CancelEventState>
{

    public State CancellationStarted { get; set; }

    public State PaymentRefunded { get; set; }
    
    public State TicketsArchived { get; set; }

    public Event<EventCanceledIntegrationEvent> EventCanceled { get; set; }

    public Event<EventPaymentsRefundedIntegrationEvent> EventPaymentRefunded { get; set; }

    public Event<EventTicketsArchivedIntegrationEvent> EventTicketsArchived { get; set; }

    public Event EventCancellationCompleted { get; set; }
    
    public CancelEventSaga(IDateTimeProvider dateTimeProvider)
    {
        // setup correlation id for the defined events
        Event(() => EventCanceled, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => EventPaymentRefunded, x => x.CorrelateById(m => m.Message.EventId));
        Event(() => EventTicketsArchived, x => x.CorrelateById(m => m.Message.EventId));
        
        // setup field state will be stored
        InstanceState(s => s.CurrentState);
        
        Initially(
            When(EventCanceled)
                .Publish(context => new EventCancellationStartedIntegrationEvent(
                        context.Message.Id,
                        context.Message.OccurredOnUtc,
                        context.Message.EventId
                    ))
                .TransitionTo(CancellationStarted)
            );
        
        During(
            CancellationStarted,
            When(EventPaymentRefunded)
                .TransitionTo(PaymentRefunded),
                When(EventTicketsArchived)
                .TransitionTo(TicketsArchived)
            );  // transition to either payment refunded when an event payment refunded event is published or tickets archived when an event tickets archived event is published when in a cancellation started state
        
        During(PaymentRefunded,
            When(EventTicketsArchived)
                .TransitionTo(TicketsArchived)); // transition to tickets archived when an event tickets archived event is published when in a payment refunded state
        
        During(TicketsArchived,
            When(EventPaymentRefunded)
                .TransitionTo(PaymentRefunded)); // transition to payment refunded when an event payment refunded event is published when in a tickets archived state
        
        /*During(CancellationStarted,
            When(EventPaymentRefunded)
                .Then(context => context.Saga.CancellationCompletedStatus |= 1)
                .TransitionTo(PaymentRefunded), // Track event completion    0001
            When(EventTicketsArchived)
                .Then(context => context.Saga.CancellationCompletedStatus |= 2)
                .TransitionTo(TicketsArchived)// Track event completion with bitwise OR operation 0010   when both events occur it will be 0001 | 0010 = 0011 (3)
        );*/
        
        // define a composite event which will mark the completion of the saga
        CompositeEvent(
            () => EventCancellationCompleted,
            state => state.CancellationCompletedStatus,
            EventPaymentRefunded, EventTicketsArchived);
        
        DuringAny(
            When(EventCancellationCompleted)
                .Publish(context => new EventCancellationCompletedIntegrationEvent(
                        Guid.NewGuid(), 
                        dateTimeProvider.UtcNow, 
                        context.Saga.CorrelationId
                    ))
                .Finalize()
            );
    }
}
