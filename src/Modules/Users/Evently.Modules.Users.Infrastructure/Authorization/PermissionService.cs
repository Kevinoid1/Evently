using Evently.Common.Application.Authorization;
using Evently.Common.Domain.Abstractions;
using Evently.Modules.Users.Application.Users.GetUserPermissions;
using MediatR;

namespace Evently.Modules.Users.Infrastructure.Authorization;

public class PermissionService(ISender mediator) :IPermissionService
{
    public async Task<Result<PermissionsResponse>> GetUserPermissionsAsync(string identityId)
    {
        return await mediator.Send(new GetUserPermissionsQuery(identityId));
    }
}
