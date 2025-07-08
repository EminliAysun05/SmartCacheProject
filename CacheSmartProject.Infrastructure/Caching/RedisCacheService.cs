using CacheSmartProject.Infrastructure.Caching.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CacheSmartProject.Infrastructure.Caching;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _database;
    private readonly IConnectionMultiplexer _connectionMultiplexer;

    public RedisCacheService(IDatabase database, IConnectionMultiplexer connectionMultiplexer)
    {
        _database = database;
        _connectionMultiplexer = connectionMultiplexer;
    }

    private static string GetModuleKey(string module) => $"cache:{module.ToLower()}";
    private static string GetLastModifiedKey(string module) => $"last_modified:{module.ToLower()}";

    public async Task<T?> GetAsync<T>(string key)
    {
        RedisValue redisValue = await _database.StringGetAsync(key);

        if (!redisValue.HasValue)
            return default;

        T? value = JsonSerializer.Deserialize<T>(redisValue);
        return value;

    }

    public async Task<DateTime?> GetLastModifiedAsync(string moduleKey)
    {
        string key = GetLastModifiedKey(moduleKey);
        RedisValue redisValue = await _database.StringGetAsync(key);

        if (!redisValue.HasValue)
        {
            return null;
        }

        DateTime parsed = DateTime.Parse(redisValue);
        return parsed;
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync(key);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
    {
        var json = JsonSerializer.Serialize(value);
        TimeSpan expiration = ttl ?? TimeSpan.FromMinutes(10);

        await _database.StringSetAsync(key, json, expiration);
    }

    public async Task SetLastModified(string moduleKey, DateTime lastModified)
    {
        string key = GetLastModifiedKey(moduleKey);
        string formattedDate = lastModified.ToString("O");

        await _database.StringSetAsync(key, formattedDate);
    }
}
