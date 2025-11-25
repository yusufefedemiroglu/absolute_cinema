using System.Text.Json;
using Application.Abstractions.Caching;
using StackExchange.Redis;

namespace Infrastructure.Cache;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;
    private readonly IConnectionMultiplexer _mux;

    public RedisCacheService(IConnectionMultiplexer mux)
    {
        _mux = mux;
        _db = mux.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);

        await _db.StringSetAsync(
            key,
            json,
            expiry ?? TimeSpan.FromMinutes(5)
        );
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

    public async Task RemoveByPatternAsync(string pattern)
    {
        var server = GetServer();

        // pattern: "products:" -> products:* 
        var keys = server.Keys(pattern: $"{pattern}*");

        foreach (var key in keys)
        {
            await _db.KeyDeleteAsync(key);
        }
    }

    private IServer GetServer()
    {
        var endpoint = _mux.GetEndPoints().First();
        return _mux.GetServer(endpoint);
    }
}