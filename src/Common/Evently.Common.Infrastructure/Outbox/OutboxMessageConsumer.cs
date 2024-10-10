namespace Evently.Common.Infrastructure.Outbox;

public sealed class OutboxMessageConsumer
{
    private OutboxMessageConsumer() { }
    public Guid OutboxMessageId { get; private set; }
    public string Name { get; private set; }

    public static OutboxMessageConsumer Create(Guid outboxMessageId, string name)
    {
        return new OutboxMessageConsumer
        {
            OutboxMessageId = outboxMessageId,
            Name = name
        };
    }
}
