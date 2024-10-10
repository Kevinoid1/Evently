using System.Data;
using System.Data.Common;
using Dapper;
using Evently.Common.Application.Clock;
using Evently.Common.Application.Data;
using Evently.Common.Application.Messaging;
using Evently.Common.Domain.Abstractions;
using Evently.Common.Infrastructure.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;

namespace Evently.Modules.Attendance.Infrastructure.Outbox;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxJob(
       IServiceScopeFactory serviceScopeFactory,
       IDateTimeProvider dateTimeProvider,
       IOptions<AttendanceModuleOutboxOptions> outboxOptions,
       ILogger<ProcessOutboxJob> logger
    ) : IJob
{
    
    private const string ModuleName = "Attendance";
    
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation("{Module} - Beginning to process outbox messages", ModuleName);
        
        using IServiceScope scope = serviceScopeFactory.CreateScope();
        IDbConnectionFactory dbConnectionFactory = scope.ServiceProvider.GetRequiredService<IDbConnectionFactory>();
        IDomainEventPublisher publisher = scope.ServiceProvider.GetRequiredService<IDomainEventPublisher>();
        
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();
        await using DbTransaction transaction = await connection.BeginTransactionAsync();
        
        // retrieve outbox messages
        IReadOnlyList<OutboxMessageResponse> outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (OutboxMessageResponse outboxMessage in outboxMessages)
        {
            Exception? exception = null;
            try
            {
                IDomainEvent domainEvent =
                    JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, SerializerSettings.Instance)!;
                
                await publisher.PublishAsync(domainEvent, Application.AssemblyMarker.Assembly, context.CancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Module} - Exception while processing outbox message with id {MessageId}", ModuleName, outboxMessage.Id);
                exception = ex;
            }
            
            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }
        
        await transaction.CommitAsync();
        
        logger.LogInformation("{Module} - Completed processing outbox messages", ModuleName);
    }


    private async Task<IReadOnlyList<OutboxMessageResponse>> GetOutboxMessagesAsync(IDbConnection connection,
        IDbTransaction transaction)
    {
        string query =
            $"""
             SELECT 
                id AS {nameof(OutboxMessageResponse.Id)}, 
                content AS {nameof(OutboxMessageResponse.Content)}
             FROM users.outbox_messages
             WHERE processed_on_utc IS NULL
             ORDER BY occurred_on_utc
             LIMIT {outboxOptions.Value.BatchSize}
             FOR UPDATE
             """;

        IEnumerable<OutboxMessageResponse> outboxMessages =
            await connection.QueryAsync<OutboxMessageResponse>(query, transaction: transaction);

        return outboxMessages.ToList().AsReadOnly();
    }

    private async Task UpdateOutboxMessageAsync(IDbConnection connection, IDbTransaction transaction,
        OutboxMessageResponse outboxMessage, Exception? exception)
    {
        const string sql = """
                           UPDATE users.outbox_messages
                            SET processed_on_utc = @ProcessedOnUtc, 
                                error = @Error
                            WHERE id = @Id
                           """;
        
        await connection.ExecuteAsync(sql, new
        {
            outboxMessage.Id,
            ProcessedOnUtc = dateTimeProvider.UtcNow,
            Error = exception?.ToString()
        }, transaction: transaction);
    }

    internal sealed record OutboxMessageResponse(Guid Id, string Content);
}
