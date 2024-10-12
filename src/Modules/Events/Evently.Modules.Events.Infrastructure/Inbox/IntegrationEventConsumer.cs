using System.Data.Common;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.EventBus;
using Evently.Common.Infrastructure.Inbox;
using MassTransit;

namespace Evently.Modules.Events.Infrastructure.Inbox;

public class IntegrationEventConsumer<TIntegrationEvent>(IDbConnectionFactory dbConnectionFactory) : IConsumer<TIntegrationEvent>
    where TIntegrationEvent : IntegrationEvent

{
    public async Task Consume(ConsumeContext<TIntegrationEvent> context)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        
        TIntegrationEvent @event = context.Message;
        var inboxMessage = InboxMessage.Create(@event);
        
        const string sql = """
                           INSERT INTO events.inbox_messages
                               (id, content, type, occured_on_utc)
                           VALUES (@Id, @Content::json, @Type, @OccuredOnUtc)
                           """;
        await connection.ExecuteAsync(sql, inboxMessage);
    }
}
