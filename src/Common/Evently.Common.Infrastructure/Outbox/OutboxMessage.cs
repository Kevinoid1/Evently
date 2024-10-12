using Evently.Common.Domain.Abstractions;
using Evently.Common.Infrastructure.Serialization;
using Newtonsoft.Json;

namespace Evently.Common.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
        
    }
    public Guid Id { get; private set; }
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime OccuredOnUtc { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }

    public static OutboxMessage Create(IDomainEvent domainEvent)
    {
        var outboxMessage = new OutboxMessage
        {
            Id = domainEvent.Id,
            Content = JsonConvert.SerializeObject(domainEvent, SerializerSettings.Instance),
            Type = domainEvent.GetType().Name,
            OccuredOnUtc = domainEvent.OccurredOnUtc
        };

        return outboxMessage;
    }
}
