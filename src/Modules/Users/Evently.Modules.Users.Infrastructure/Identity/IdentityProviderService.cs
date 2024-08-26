using System.Net;
using Evently.Common.Domain.Abstractions;
using Evently.Modules.Users.Application.Abstractions.Identity;
using Microsoft.Extensions.Logging;

namespace Evently.Modules.Users.Infrastructure.Identity;

internal sealed class IdentityProviderService(KeyCloakClient keyCloakClient, ILogger<IdentityProviderService> logger) : IIdentityProviderService
{
    private const string PasswordCredentialType = "Password";

    public async Task<Result<string>> RegisterUserAsync(UserModel user, CancellationToken cancellationToken = default)
    {
        var userRepresentation = new UserRepresentation(
            user.Email,
            user.Email,
            user.FirstName,
            user.LastName,
            true,
            true,
            [new CredentialRepresentation(PasswordCredentialType, user.Password, false)]);

        try
        {
            string identityId = await keyCloakClient.RegisterUserAsync(userRepresentation, cancellationToken);

            return identityId;
        }
        catch (HttpRequestException exception) when (exception.StatusCode == HttpStatusCode.Conflict)
        {
            logger.LogError(ApplicationEventIds.UserRegistered, exception, "User registration failed");
            return Result.Failure<string>(IdentityProviderErrors.EmailIsNotUnique);
        }
    }
}
