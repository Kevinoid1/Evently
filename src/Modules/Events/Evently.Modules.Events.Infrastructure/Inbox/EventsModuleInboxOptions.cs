namespace Evently.Modules.Events.Infrastructure.Inbox;

internal sealed class EventsModuleInboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
