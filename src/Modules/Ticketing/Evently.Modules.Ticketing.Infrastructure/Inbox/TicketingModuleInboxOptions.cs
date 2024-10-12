namespace Evently.Modules.Ticketing.Infrastructure.Inbox;

internal sealed class TicketingModuleInboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
