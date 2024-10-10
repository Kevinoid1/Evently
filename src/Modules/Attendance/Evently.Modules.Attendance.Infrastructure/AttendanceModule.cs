using Evently.Common.Application.Messaging;
using Evently.Common.Infrastructure.Interceptors;
using Evently.Common.Presentation.Endpoints;
using Evently.Modules.Attendance.Application.Abstractions.Authentication;
using Evently.Modules.Attendance.Application.Abstractions.Data;
using Evently.Modules.Attendance.Domain.Attendees;
using Evently.Modules.Attendance.Domain.Events;
using Evently.Modules.Attendance.Domain.Tickets;
using Evently.Modules.Attendance.Infrastructure.Attendees;
using Evently.Modules.Attendance.Infrastructure.Authentication;
using Evently.Modules.Attendance.Infrastructure.Database;
using Evently.Modules.Attendance.Infrastructure.Events;
using Evently.Modules.Attendance.Infrastructure.Outbox;
using Evently.Modules.Attendance.Infrastructure.Tickets;
using Evently.Modules.Attendance.Presentation.Attendees;
using Evently.Modules.Attendance.Presentation.Events;
using Evently.Modules.Attendance.Presentation.Tickets;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Evently.Modules.Attendance.Infrastructure;

public static class AttendanceModule
{
    public static IServiceCollection AddAttendanceModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyMarker.Assembly);
        
        services.ConfigureBackgroundJobs(configuration);

        return services;
    }
    
    public static void ConfigureConsumers(IRegistrationConfigurator registrationConfigurator)
    {
        registrationConfigurator.AddConsumer<UserRegisteredIntegrationEventConsumer>();
        registrationConfigurator.AddConsumer<UserProfileUpdatedIntegrationEventConsumer>();
        registrationConfigurator.AddConsumer<EventPublishedIntegrationEventConsumer>();
        registrationConfigurator.AddConsumer<TicketIssuedIntegrationEventConsumer>();
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AttendanceDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Attendance))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AttendanceDbContext>());

        services.AddScoped<IAttendeeRepository, AttendeeRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<ITicketRepository, TicketRepository>();

        services.AddScoped<IAttendanceContext, AttendanceContext>();
        
        services.AddDomainEventHandlers();
    }
    
    private static void ConfigureBackgroundJobs(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<AttendanceModuleOutboxOptions>(configuration.GetSection("Attendance:Outbox"));
        services.ConfigureOptions<ConfigureProcessOutboxJob>();
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
}
