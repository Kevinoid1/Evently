using Evently.Common.Application.Authorization;
using Evently.Common.Application.EventBus;
using Evently.Common.Application.Messaging;
using Evently.Common.Infrastructure.Interceptors;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Users.Application.Abstractions.Data;
using Evently.Modules.Users.Application.Abstractions.Identity;
using Evently.Modules.Users.Domain.Users;
using Evently.Modules.Users.Infrastructure.Authorization;
using Evently.Modules.Users.Infrastructure.Database;
using Evently.Modules.Users.Infrastructure.Identity;
using Evently.Modules.Users.Infrastructure.Inbox;
using Evently.Modules.Users.Infrastructure.Outbox;
using Evently.Modules.Users.Infrastructure.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Evently.Modules.Users.Infrastructure;

public static class UsersModule
{
    public static IServiceCollection AddUsersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .ConfigureKeyCloak(configuration)
            .AddInfrastructure(configuration);

        services.ConfigureBackgroundJobs(configuration);

        services.AddEndpoints(Presentation.AssemblyMarker.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string databaseConnectionString = configuration.GetConnectionString("Database")!;
        services.AddDbContext<UsersDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    databaseConnectionString,
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Users))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<UsersDbContext>());

        services.AddScoped<IPermissionService, PermissionService>();
        
        services.AddDomainEventHandlers();
        
        services.AddIntegrationEventHandlers();
    }

    private static IServiceCollection ConfigureKeyCloak(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<KeyCloakOptions>(configuration.GetSection("Users:KeyCloak"));
        services.AddTransient<KeyCloakAuthDelegatingHandler>();

        services.AddHttpClient<KeyCloakClient>((sp, httpClient) =>
            {
                KeyCloakOptions keyCloakOptions = sp.GetRequiredService<IOptions<KeyCloakOptions>>().Value;
                httpClient.BaseAddress = new Uri(keyCloakOptions.AdminUrl);
            })
            .AddHttpMessageHandler<KeyCloakAuthDelegatingHandler>(); // add delegating handler

        services.AddTransient<IIdentityProviderService, IdentityProviderService>();
        
        return services;
    }

    private static void ConfigureBackgroundJobs(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<UserModuleOutboxOptions>(configuration.GetSection("Users:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
        services.Configure<UserModuleInboxOptions>(configuration.GetSection("Users:Inbox"));
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
            services.Decorate(domainEventHandler, idempotentDomainEventHandler); // decorating the domain event handler type with IdempotentDomainEventHandler
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
