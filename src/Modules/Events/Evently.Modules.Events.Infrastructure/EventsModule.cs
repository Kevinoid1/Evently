using Evently.Common.Application.EventBus;
using Evently.Common.Application.Messaging;
using Evently.Common.Infrastructure.Interceptors;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Events.Application.Abstraction.Data;
using Evently.Modules.Events.Domain.Categories;
using Evently.Modules.Events.Domain.Events;
using Evently.Modules.Events.Domain.TicketTypes;
using Evently.Modules.Events.Infrastructure.Categories;
using Evently.Modules.Events.Infrastructure.Database;
using Evently.Modules.Events.Infrastructure.Events;
using Evently.Modules.Events.Infrastructure.Inbox;
using Evently.Modules.Events.Infrastructure.Outbox;
using Evently.Modules.Events.Infrastructure.TicketTypes;
using Evently.Modules.Events.Presentation.Events.CancelEventSaga;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Evently.Modules.Events.Infrastructure;

public static class EventsModule
{
    public static IServiceCollection AddEventsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddEndpoints(Presentation.AssemblyMarker.Assembly);
        services
            .AddInfrastructure(configuration);
        
        services.ConfigureBackgroundJobs(configuration);

        return services;
    }

    public static Action<IRegistrationConfigurator> ConfigureConsumers(string redisConnectionString)
    {
        return configurator => configurator.AddSagaStateMachine<CancelEventSaga, CancelEventState>()
            .RedisRepository(redisConnectionString);
    }
    
    private static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string databaseConnectionString = configuration.GetConnectionString("Database")!;

        services.AddDbContext<EventsDbContext>((sp,options) =>
            options
                .UseNpgsql(
                    databaseConnectionString,
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Events))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));
        
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketTypeRepository, TicketTypeRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<EventsDbContext>());
        
        services.AddDomainEventHandlers();
        
        services.AddIntegrationEventHandlers();
        
        return services;
    }
    
    private static void ConfigureBackgroundJobs(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EventsModuleOutboxOptions>(configuration.GetSection("Events:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>(); 
        services.Configure<EventsModuleInboxOptions>(configuration.GetSection("Events:Inbox"));
        services.ConfigureOptions<ConfigureProcessInboxJob>();
    }
    
    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyMarker.Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler))).ToArray(); // gets all the domain event handlers that assignable to IDomainEventHandler

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            Type domainEvent = domainEventHandler.GetInterfaces()
                .Single(@interface => @interface.IsGenericType)
                .GetGenericArguments()
                .Single();  // this gives me the domain event which is the generic parameter
            
            Type idempotentDomainEventHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);
            
            // from scrutor
            services.Decorate(domainEventHandler, idempotentDomainEventHandler);
        }
    }
    
    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        Type[] integrationEventHandlers = Presentation.AssemblyMarker.Assembly.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler))).ToArray(); // gets all the integration event handlers that assignable to IIntegrationEventHandler

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.TryAddScoped(integrationEventHandler);

            Type integrationEvent = integrationEventHandler.GetInterfaces()
                .Single(@interface => @interface.IsGenericType)
                .GetGenericArguments()
                .Single();  // this gives me the integration event which is the generic parameter
            
            Type idempotentIntegrationEventHandler = typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);
            
            // from scrutor
            services.Decorate(integrationEventHandler, idempotentIntegrationEventHandler); // decorating the integration event handler type with IdempotentIntegrationEventHandler
        }
    }
}
