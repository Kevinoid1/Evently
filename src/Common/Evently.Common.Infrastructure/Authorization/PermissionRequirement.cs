using Microsoft.AspNetCore.Authorization;

namespace Evently.Common.Infrastructure.Authorization;

internal sealed class PermissionRequirement : IAuthorizationRequirement
{
    public IEnumerable<string>  Permissions { get; }

    public PermissionRequirement(IEnumerable<string> permissions)
    {
        Permissions = permissions;
    }
}
