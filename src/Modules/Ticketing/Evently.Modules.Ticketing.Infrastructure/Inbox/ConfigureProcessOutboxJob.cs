using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Ticketing.Infrastructure.Inbox;

internal sealed class ConfigureProcessInboxJob(IOptions<TicketingModuleInboxOptions> inboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly TicketingModuleInboxOptions _ticketingModuleInboxOptions = inboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessInboxJob).FullName!;

        options.AddJob<ProcessInboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_ticketingModuleInboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
