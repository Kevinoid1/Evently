using Evently.Modules.Events.Application.Events.GetEvent;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Evently.Modules.Events.Presentation.Events;

public static class GetEvent
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("events/{id}", async (Guid id, ISender mediator) =>
            {
                var query = new GetEventQuery(id);
                EventResponse? @event= await mediator.Send(query);

                return @event is null ? Results.NotFound() : Results.Ok(@event);
            })
        .WithTags(Tags.Events);
    }
}
