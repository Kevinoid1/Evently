using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Evently.Modules.Events.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // mediatR registration
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Application.AssemblyMarker.Assembly);
        });
        
        // fluent validation registration
        services.AddValidatorsFromAssembly(Application.AssemblyMarker.Assembly);
        
        return services;
    }
}
