using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Events.Infrastructure.Outbox;

internal sealed class ConfigureProcessOutboxJob(IOptions<EventsModuleOutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly EventsModuleOutboxOptions _eventsModuleOutboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessOutboxJob).FullName!;

        options.AddJob<ProcessOutboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_eventsModuleOutboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
