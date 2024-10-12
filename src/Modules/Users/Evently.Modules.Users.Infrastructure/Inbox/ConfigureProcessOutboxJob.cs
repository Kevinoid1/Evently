using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Users.Infrastructure.Inbox;

internal sealed class ConfigureProcessInboxJob(IOptions<UserModuleInboxOptions> inboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly UserModuleInboxOptions _userModuleInboxOptions = inboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessInboxJob).FullName!;

        options.AddJob<ProcessInboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_userModuleInboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
