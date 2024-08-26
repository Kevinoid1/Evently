using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Evently.Common.Application.Caching;
using Microsoft.Extensions.Options;
using Serilog;

namespace Evently.Modules.Users.Infrastructure.Identity;

internal sealed class KeyCloakAuthDelegatingHandler(IOptions<KeyCloakOptions> options, ICacheService cacheService) : DelegatingHandler
{
    private readonly KeyCloakOptions _keyCloakOptions = options.Value;
    private const string TokenCacheKey = "keycloak_client_token";

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        AuthToken authToken = await GetTokenAsync(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken.AccessToken);
        HttpResponseMessage httpResponseMessage = await base.SendAsync(request, cancellationToken);

        string response = await httpResponseMessage.Content.ReadAsStringAsync(cancellationToken);
        Log.Information("KeyCloak response: {Response}", response);
        httpResponseMessage.EnsureSuccessStatusCode();
        return httpResponseMessage;
    }

    private async Task<AuthToken> GetTokenAsync(CancellationToken cancellationToken)
    {
        AuthToken token = await cacheService.GetAsync<AuthToken>(TokenCacheKey, cancellationToken);
        if (token is not null)
        {
            return token;
        }

        // call auth token and cache it
        token = await GetAuthorizationTokenAsync(cancellationToken);
            
        // set cache expiration to 2 minutes before the token expires
        var expiration = TimeSpan.FromSeconds(token.ExpiresIn - 120);
        await cacheService.SetAsync(TokenCacheKey, token, expiration,cancellationToken);

        return token;
    }


    private async Task<AuthToken> GetAuthorizationTokenAsync(CancellationToken cancellationToken)
    {
        KeyValuePair<string,string>[] authRequestParameters = [
            new KeyValuePair<string, string>("client_id", _keyCloakOptions.ConfidentialClientId),
            new KeyValuePair<string, string>("client_secret", _keyCloakOptions.ConfidentialClientSecret),
            new KeyValuePair<string, string>("scope", "openid"),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
        ];

        using var authRequestContent = new FormUrlEncodedContent(authRequestParameters);
        using var authRequest = new HttpRequestMessage(HttpMethod.Post, new Uri(_keyCloakOptions.TokenUrl));
        authRequest.Content = authRequestContent;

        using HttpResponseMessage authResponse = await base.SendAsync(authRequest, cancellationToken);

        authResponse.EnsureSuccessStatusCode();
        return await authResponse.Content.ReadFromJsonAsync<AuthToken>(cancellationToken: cancellationToken);
    }

    internal sealed class AuthToken
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }
        [JsonPropertyName("expires_in")]
        public long ExpiresIn { get; init; }
    }
}
