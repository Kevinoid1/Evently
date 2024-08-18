using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Presentation.Events;

public static class EventsEndpoints
{
    public static void MapEndpoints(IEndpointRouteBuilder app)
    {
        CreateEvent.MapEndpoint(app);
        GetEvent.MapEndpoint(app);
        CancelEvent.MapEndpoint(app);
        RescheduleEvent.MapEndpoint(app);
        PublishEvent.MapEndpoint(app);
        GetEvents.MapEndpoint(app);
    }
}
