using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Ticketing.Infrastructure.Outbox;

internal sealed class ConfigureProcessOutboxJob(IOptions<TicketingModuleOutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly TicketingModuleOutboxOptions _ticketingModuleOutboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessOutboxJob).FullName!;

        options.AddJob<ProcessOutboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_ticketingModuleOutboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
