using System.Data.Common;
using Dapper;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Evently.Common.Infrastructure.Outbox;

namespace Evently.Modules.Attendance.Infrastructure.Outbox;

internal sealed class IdempotentDomainEventHandler<TDomainEvent>(
    IDomainEventHandler<TDomainEvent> decoratedHandler,
    IDbConnectionFactory dbConnectionFactory
    ) : DomainEventHandler<TDomainEvent>
    where TDomainEvent : IDomainEvent
{
    public override async Task Handle(TDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        
        var outboxMessageConsumer = OutboxMessageConsumer.Create(domainEvent.Id, typeof(TDomainEvent).Name);

        if (await OutboxMessageConsumerExistsAsync(connection, outboxMessageConsumer))
        {
            return;
        }
        
        await decoratedHandler.Handle(domainEvent, cancellationToken);
        
        await InsertOutboxMessageConsumerAsync(connection, outboxMessageConsumer);
    }

    private static async Task<bool> OutboxMessageConsumerExistsAsync(DbConnection connection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        const string sql =
            """
                SELECT EXISTS (
                    SELECT 1 
                    FROM attendance.outbox_message_consumers 
                    WHERE outbox_message_id = @OutboxMessageId AND name = @Name)
            """;

        return await connection.ExecuteScalarAsync<bool>(sql, outboxMessageConsumer);
    }
    
    private static async Task InsertOutboxMessageConsumerAsync(DbConnection connection,
        OutboxMessageConsumer outboxMessageConsumer)
    {
        const string sql =
            """
                INSERT INTO attendance.outbox_message_consumers
                    (outbox_message_id, name)
                VALUES
                    (@OutboxMessageId, @Name)
            """;
        
        await connection.ExecuteAsync(sql, outboxMessageConsumer);
    }
}
