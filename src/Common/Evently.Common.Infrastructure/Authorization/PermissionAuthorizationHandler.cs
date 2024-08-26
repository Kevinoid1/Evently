using Evently.Common.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace Evently.Common.Infrastructure.Authorization;

internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        HashSet<string> permissions = context.User.GetPermissions();

        if (requirement.Permissions.Any(p => permissions.Contains(p)))
        {
            context.Succeed(requirement);
        }

        /*if (!requiredPermissions.Except(permissions).Any())
            context.Succeed(requirement)
        */

        return Task.CompletedTask;
    }
}
