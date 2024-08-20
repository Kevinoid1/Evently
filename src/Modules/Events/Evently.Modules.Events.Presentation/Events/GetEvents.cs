using Evently.Common.Domain.Abstractions;
using Evently.Common.Presentation.ApiResults;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Events.Application.Events.GetEvents;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using EventResponse = Evently.Modules.Events.Application.Events.GetEvents.EventResponse;

namespace Evently.Modules.Events.Presentation.Events;

internal class GetEvents : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("events", async (ISender mediator) =>
            {
                Result<IReadOnlyCollection<EventResponse>> result = await mediator.Send(new GetEventsQuery());

                return result.Match(Results.Ok, ApiResults.Problem);
            })
            .WithTags(Tags.Events);
    }
}
