namespace Evently.Common.Application.Authorization;

public record PermissionsResponse(Guid UserId, HashSet<string> Permissions);
