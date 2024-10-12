using System.Data.Common;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.EventBus;
using Evently.Common.Infrastructure.Inbox;

namespace Evently.Modules.Ticketing.Infrastructure.Inbox;

internal sealed class IdempotentIntegrationEventHandler<TIntegrationEvent>(
    IIntegrationEventHandler<TIntegrationEvent> decoratedHandler,
    IDbConnectionFactory dbConnectionFactory
    ) : IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
    public override async Task Handle(TIntegrationEvent integrationEvent, CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        
        var inboxMessageConsumer = InboxMessageConsumer.Create(integrationEvent.Id, typeof(TIntegrationEvent).Name);

        if (await InboxMessageConsumerExistsAsync(connection, inboxMessageConsumer))
        {
            return;
        }
        
        await decoratedHandler.Handle(integrationEvent, cancellationToken);
        
        await InsertInboxMessageConsumerAsync(connection, inboxMessageConsumer);
    }

    private static async Task<bool> InboxMessageConsumerExistsAsync(DbConnection connection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            """
                SELECT EXISTS (
                    SELECT 1 
                    FROM ticketing.inbox_message_consumers 
                    WHERE inbox_message_id = @InboxMessageId AND name = @Name)
            """;

        return await connection.ExecuteScalarAsync<bool>(sql, inboxMessageConsumer);
    }
    
    private static async Task InsertInboxMessageConsumerAsync(DbConnection connection,
        InboxMessageConsumer inboxMessageConsumer)
    {
        const string sql =
            """
                INSERT INTO ticketing.inbox_message_consumers
                    (inbox_message_id, name)
                VALUES
                    (@InboxMessageId, @Name)
            """;
        
        await connection.ExecuteAsync(sql, inboxMessageConsumer);
    }
}
