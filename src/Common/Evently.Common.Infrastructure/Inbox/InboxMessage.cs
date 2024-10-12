using Evently.Common.Application.EventBus;
using Evently.Common.Infrastructure.Serialization;
using Newtonsoft.Json;

namespace Evently.Common.Infrastructure.Inbox;

public sealed class InboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime OccuredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }

    public static InboxMessage Create(IIntegrationEvent integrationEvent)
    {
        var inboxMessage = new InboxMessage
        {
            Id = integrationEvent.Id,
            Content = JsonConvert.SerializeObject(integrationEvent, SerializerSettings.Instance),
            Type = integrationEvent.GetType().Name,
            OccuredOnUtc = integrationEvent.OccurredOnUtc
        };

        return inboxMessage;
    }
}
