using Evently.Common.Application.Caching;
using Evently.Common.Application.Clock;
using Evently.Common.Application.Data;
using Evently.Common.Application.EventBus;
using Evently.Common.Application.Messaging;
using Evently.Common.Infrastructure.Authentication;
using Evently.Common.Infrastructure.Authorization;
using Evently.Common.Infrastructure.Caching;
using Evently.Common.Infrastructure.Clock;
using Evently.Common.Infrastructure.Data;
using Evently.Common.Infrastructure.Interceptors;
using Evently.Common.Infrastructure.Outbox;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using Quartz;
using StackExchange.Redis;

namespace Evently.Common.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<IRegistrationConfigurator>[] configurators, string databaseConnectionString, string redisConnectionString)
    {
        services.AddHttpContextAccessor();
        
        // setup authentication and authorization
        services.AddAuthenticationInternal()
            .AddAuthorizationInternal();
        
        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        
        services.TryAddSingleton(dataSource);
        services.TryAddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        services.TryAddSingleton<InsertOutboxMessagesInterceptor>();
        
        // register my custom publisher
        services.TryAddScoped<IDomainEventPublisher, DomainEventPublisher>();

        services
            .ConfigureCaching(redisConnectionString)
            .ConfigureMassTransit(configurators)
            .RegisterQuartzServices();
        
        return services;
    }
    
    private static IServiceCollection ConfigureCaching(this IServiceCollection services, string redisConnectionString)
    {
        try
        {
            IConnectionMultiplexer connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            services.TryAddSingleton(connectionMultiplexer);

            services.AddStackExchangeRedisCache(options =>
            {
                options.ConnectionMultiplexerFactory = () => Task.FromResult(connectionMultiplexer);
            });
        }
        catch 
        {
            services.AddDistributedMemoryCache();
        }
        
        services.TryAddSingleton<ICacheService, CacheService>();
        
        return services;
    }

    private static IServiceCollection ConfigureMassTransit(this IServiceCollection services, Action<IRegistrationConfigurator>[] configurators)
    {
        services.TryAddSingleton<IEventBus, EventBus.EventBus>();

        services.AddMassTransit(configurator =>
        {
            foreach (Action<IRegistrationConfigurator> consumerConfigurator in configurators)
            {
                consumerConfigurator(configurator);
            }
            configurator.SetKebabCaseEndpointNameFormatter();
            configurator.UsingInMemory((context, cfg) =>
            {
                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }

    private static IServiceCollection RegisterQuartzServices(this IServiceCollection services)
    {
        services.AddQuartz();
        
        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}
