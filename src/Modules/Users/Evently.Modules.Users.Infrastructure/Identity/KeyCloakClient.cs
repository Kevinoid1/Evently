using System.Net.Http.Json;

namespace Evently.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakClient(HttpClient client)
{
    internal async Task<string> RegisterUserAsync(UserRepresentation user,
        CancellationToken cancellationToken = default)
    {
        HttpResponseMessage httpResponseMessage = await client.PostAsJsonAsync("users", user, cancellationToken);
        return ExtractIdentityIdFromLocationHeader(httpResponseMessage);
    }

    private string ExtractIdentityIdFromLocationHeader(HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";
        string locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery;
        if (locationHeader is null)
        {
            throw new InvalidOperationException("Location header is null");
        }
        
        int userSegmentIndex = locationHeader.IndexOf(usersSegmentName, StringComparison.InvariantCultureIgnoreCase);
        string identityId = locationHeader.Substring(userSegmentIndex + usersSegmentName.Length);
        return identityId;
    }
}
