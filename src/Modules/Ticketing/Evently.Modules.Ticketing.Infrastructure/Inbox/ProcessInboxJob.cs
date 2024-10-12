using System.Data;
using System.Data.Common;
using Dapper;
using Evently.Common.Application.Clock;
using Evently.Common.Application.Data;
using Evently.Common.Application.EventBus;
using Evently.Common.Application.Messaging;
using Evently.Common.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Evently.Modules.Ticketing.Infrastructure.Inbox;

[DisallowConcurrentExecution]
internal sealed class ProcessInboxJob(
       IServiceScopeFactory serviceScopeFactory,
       IDateTimeProvider dateTimeProvider,
       IOptions<TicketingModuleInboxOptions> inboxOptions,
       ILogger<ProcessInboxJob> logger
    ) : IJob
{
    
    private const string ModuleName = "Ticketing";
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process inbox messages", ModuleName);
        
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IDbConnectionFactory dbConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        IEventPublisher publisher = scope.ServiceProvider.GetRequiredService<IEventPublisher>();
        
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();
        
        // retrieve outbox messages
        IReadOnlyList<InboxMessageResponse> inboxMessages = await GetInboxMessagesAsync(connection, transaction);

        foreach (InboxMessageResponse inboxMessage in inboxMessages)
        {
            Exception? exception = null;
            try
            {
                IIntegrationEvent integrationEvent =
                    JsonConvert.DeserializeObject<IIntegrationEvent>(inboxMessage.Content, SerializerSettings.Instance)!;
                
                await publisher.PublishIntegrationEventAsync(integrationEvent, Presentation.AssemblyMarker.Assembly, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Module} - Exception while processing inbox message with id {MessageId}", ModuleName, inboxMessage.Id);
                exception = ex;
            }
            
            await UpdateInboxMessageAsync(connection, transaction, inboxMessage, exception);
        }
        
        await transaction.CommitAsync();
        
        logger.LogInformation("{Module} - Completed processing inbox messages", ModuleName);
    }


    private async Task<IReadOnlyList<InboxMessageResponse>> GetInboxMessagesAsync(IDbConnection connection,
        IDbTransaction transaction)
    {
        string query =
            $"""
             SELECT 
                id AS {nameof(InboxMessageResponse.Id)}, 
                content AS {nameof(InboxMessageResponse.Content)}
             FROM ticketing.inbox_messages
             WHERE processed_on_utc IS NULL
             ORDER BY occured_on_utc
             LIMIT {inboxOptions.Value.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<InboxMessageResponse> inboxMessages =
            await connection.QueryAsync<InboxMessageResponse>(query, transaction: transaction);

        return inboxMessages.ToList().AsReadOnly();
    }

    private async Task UpdateInboxMessageAsync(IDbConnection connection, IDbTransaction transaction,
        InboxMessageResponse inboxMessage, Exception? exception)
    {
        const string sql = """
                           UPDATE ticketing.inbox_messages
                            SET processed_on_utc = @ProcessedOnUtc, 
                                error = @Error
                            WHERE id = @Id
                           """;
        
        await connection.ExecuteAsync(sql, new
        {
            inboxMessage.Id,
            ProcessedOnUtc = dateTimeProvider.UtcNow,
            Error = exception?.ToString()
        }, transaction: transaction);
    }

    internal sealed record InboxMessageResponse(Guid Id, string Content);
}
