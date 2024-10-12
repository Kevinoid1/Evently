using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Attendance.Infrastructure.Inbox;

internal sealed class ConfigureProcessInboxJob(IOptions<AttendanceModuleInboxOptions> inboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly AttendanceModuleInboxOptions _attendanceModuleInboxOptions = inboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessInboxJob).FullName!;

        options.AddJob<ProcessInboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_attendanceModuleInboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
