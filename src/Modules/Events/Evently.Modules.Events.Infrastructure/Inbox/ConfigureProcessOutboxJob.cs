using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Events.Infrastructure.Inbox;

internal sealed class ConfigureProcessInboxJob(IOptions<EventsModuleInboxOptions> inboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly EventsModuleInboxOptions _eventsModuleInboxOptions = inboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessInboxJob).FullName!;

        options.AddJob<ProcessInboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_eventsModuleInboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
