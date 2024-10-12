namespace Evently.Modules.Attendance.Infrastructure.Inbox;

internal sealed class AttendanceModuleInboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
