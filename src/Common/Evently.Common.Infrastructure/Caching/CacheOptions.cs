using Microsoft.Extensions.Caching.Distributed;

namespace Evently.Common.Infrastructure.Caching;

internal static class CacheOptions
{
    private static DistributedCacheEntryOptions DefaultExpiration => new() {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2)};
    
    internal static DistributedCacheEntryOptions Create(TimeSpan? expiration) =>
        expiration is not null
            ? new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            }
            : DefaultExpiration;
}
