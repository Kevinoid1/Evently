namespace Evently.Modules.Users.Infrastructure.Inbox;

internal sealed class UserModuleInboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
