using Evently.Modules.Events.Application.Events.GetEvents;
using Evently.Modules.Events.Domain.Abstractions;
using Evently.Modules.Events.Presentation.ApiResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using EventResponse = Evently.Modules.Events.Application.Events.GetEvents.EventResponse;

namespace Evently.Modules.Events.Presentation.Events;

internal static class GetEvents
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("events", async (ISender mediator) =>
            {
                Result<IReadOnlyCollection<EventResponse>> result = await mediator.Send(new GetEventsQuery());

                return result.Match(Results.Ok, ApiResults.ApiResults.Problem);
            })
            .WithTags(Tags.Events);
    }
}
