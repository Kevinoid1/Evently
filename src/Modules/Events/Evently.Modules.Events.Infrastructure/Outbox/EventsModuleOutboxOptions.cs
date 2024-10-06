namespace Evently.Modules.Events.Infrastructure.Outbox;

internal sealed class EventsModuleOutboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
