using Evently.Common.Application.Caching;
using Evently.Common.Application.Clock;
using Evently.Common.Application.Data;
using Evently.Common.Application.EventBus;
using Evently.Common.Infrastructure.Caching;
using Evently.Common.Infrastructure.Clock;
using Evently.Common.Infrastructure.Data;
using Evently.Common.Infrastructure.Interceptors;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Npgsql;
using StackExchange.Redis;

namespace Evently.Common.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, Action<IRegistrationConfigurator>[] configurators, string databaseConnectionString, string redisConnectionString)
    {
        NpgsqlDataSource dataSource = new NpgsqlDataSourceBuilder(databaseConnectionString).Build();
        
        services.TryAddSingleton(dataSource);
        services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();
        services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
        
        services.TryAddSingleton<PublishDomainEventsInterceptor>();

        services
            .ConfigureCaching(redisConnectionString)
            .ConfigureMassTransit(configurators);
        
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
}
