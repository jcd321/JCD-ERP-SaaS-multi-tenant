using System.Text.Json;
using Jcd.Erp.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;

namespace Jcd.Erp.Infrastructure.Cache;

public sealed class RedisCacheService : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly IDistributedCache _cache;
    private readonly IConnectionMultiplexer _redis;
    private readonly string _instancePrefix;

    public RedisCacheService(
        IDistributedCache cache,
        IConnectionMultiplexer redis,
        RedisCacheOptions options)
    {
        _cache = cache;
        _redis = redis;
        _instancePrefix = options.InstanceName;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var payload = await _cache.GetStringAsync(key, cancellationToken);
        if (string.IsNullOrWhiteSpace(payload))
            return default;

        return JsonSerializer.Deserialize<T>(payload, JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan ttl,
        CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(value, JsonOptions);
        await _cache.SetStringAsync(
            key,
            payload,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl,
            },
            cancellationToken);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) =>
        _cache.RemoveAsync(key, cancellationToken);

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        var pattern = $"{_instancePrefix}{prefix}*";
        var database = _redis.GetDatabase();

        foreach (var endpoint in _redis.GetEndPoints())
        {
            var server = _redis.GetServer(endpoint);
            if (!server.IsConnected || server.IsReplica)
                continue;

            await foreach (var key in server.KeysAsync(pattern: pattern).WithCancellation(cancellationToken))
            {
                var logicalKey = key.ToString();
                if (logicalKey.StartsWith(_instancePrefix, StringComparison.Ordinal))
                    logicalKey = logicalKey[_instancePrefix.Length..];

                await _cache.RemoveAsync(logicalKey, cancellationToken);
            }
        }
    }
}

public sealed class RedisCacheOptions
{
    public string InstanceName { get; init; } = "jcd_erp:";
}
