using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Evently.Common.Infrastructure.Authentication;

internal static class AuthenticationExtension
{
    internal static IServiceCollection AddAuthenticationInternal(this IServiceCollection services)
    {
        services.AddSingleton<HandleFailedAuthenticationEvent>();
        services.AddAuthentication().AddJwtBearer();
        services.ConfigureOptions<JwtBearerConfigureOptions>();
        return services;
    }
}
