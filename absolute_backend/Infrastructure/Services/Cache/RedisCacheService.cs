using System.Text.Json;
using Application.Abstractions;
using Application.Abstractions.Caching;
using StackExchange.Redis;

namespace Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        var expiration = expiry.HasValue
            ? new Expiration(expiry.Value)
            : new Expiration(TimeSpan.FromMinutes(5));

        await _db.StringSetAsync(key, json, expiration);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _db.StringGetAsync(key);
        if (value.IsNullOrEmpty)
            return default;

        return JsonSerializer.Deserialize<T>(value!);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}