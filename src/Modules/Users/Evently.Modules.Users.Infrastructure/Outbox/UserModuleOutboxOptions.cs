namespace Evently.Modules.Users.Infrastructure.Outbox;

internal sealed class UserModuleOutboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
