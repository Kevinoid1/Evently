using Evently.Common.Application.Authorization;
using Evently.Common.Application.Messaging;

namespace Evently.Modules.Users.Application.Users.GetUserPermissions;

public record GetUserPermissionsQuery(string IdentityId) : IQuery<PermissionsResponse>;
