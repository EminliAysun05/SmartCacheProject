namespace CacheSmartProject.Infrastructure.Caching.Interfaces;

public interface IRedisCacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? ttl = null);
    Task RemoveAsync(string key);

    Task<DateTime?> GetLastModifiedAsync(string moduleKey);
    Task SetLastModified(string moduleKey, DateTime lastModified);
}
