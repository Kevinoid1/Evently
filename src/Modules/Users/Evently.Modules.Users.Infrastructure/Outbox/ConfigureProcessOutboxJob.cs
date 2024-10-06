using Microsoft.Extensions.Options;
using Quartz;

namespace Evently.Modules.Users.Infrastructure.Outbox;

internal sealed class ConfigureProcessOutboxJob(IOptions<UserModuleOutboxOptions> outboxOptions) : IConfigureOptions<QuartzOptions>
{
    private readonly UserModuleOutboxOptions _userModuleOutboxOptions = outboxOptions.Value;

    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessOutboxJob).FullName!;

        options.AddJob<ProcessOutboxJob>(builder => builder.WithIdentity(jobName))
            .AddTrigger(triggerBuilder => triggerBuilder.ForJob(jobName)
                .WithSimpleSchedule(schedule => schedule.WithIntervalInSeconds(_userModuleOutboxOptions.IntervalInSeconds)
                    .RepeatForever()));
    }
}
