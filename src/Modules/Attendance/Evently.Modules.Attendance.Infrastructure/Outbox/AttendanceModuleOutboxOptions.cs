namespace Evently.Modules.Attendance.Infrastructure.Outbox;

internal sealed class AttendanceModuleOutboxOptions
{
    public int IntervalInSeconds { get; init; }
    public int BatchSize { get; init; }
}
