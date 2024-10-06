namespace Evently.Modules.Ticketing.Infrastructure.Outbox;

internal sealed class TicketingModuleOutboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
