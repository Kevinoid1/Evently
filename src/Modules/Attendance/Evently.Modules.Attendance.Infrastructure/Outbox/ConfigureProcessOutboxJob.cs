using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Attendance.Infrastructure.Outbox;

internal sealed class ConfigureProcessOutboxJob(IOptions<AttendanceModuleOutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly AttendanceModuleOutboxOptions _attendanceModuleOutboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessOutboxJob).FullName!;

        options.AddJob<ProcessOutboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_attendanceModuleOutboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
