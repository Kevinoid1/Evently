namespace Evently.Common.Infrastructure.Inbox;

public sealed class InboxMessageConsumer
{
    private InboxMessageConsumer(){} 
    public Guid InboxMessageId { get; private set; }
    public string Name { get; private set; }

    public static InboxMessageConsumer Create(Guid inboxMessageId, string name)
    {
        return new InboxMessageConsumer
        {
            InboxMessageId = inboxMessageId,
            Name = name
        };
    }
}
