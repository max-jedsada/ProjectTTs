using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Project.Manager.Exception;
using StackExchange.Redis;

namespace Project.Manager.Redis;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDistributedCache? _cache;
    private readonly IConfiguration? _configuration;

    public RedisCacheService(IDistributedCache cache, IConfiguration? configuration)
    {
        _cache = cache;
        _configuration = configuration;
    }

    public string GetCachedData(string key)
    {
        var jsonData = _cache.GetString(key);

        if (jsonData == null)
            return "";

        return jsonData;
    }

    public void SetCachedData(string key, string dataJson, TimeSpan cacheDuration)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheDuration
        };

        _cache.SetString(key, dataJson, options);
    }

    public void Remove(string key)
    {
        _cache.Remove(key);

    }

    public async Task<List<string>> GetAll(string keyScan)
    {
        var serverConfig = _configuration.GetConnectionString("redisConn");

        ConfigurationOptions options = ConfigurationOptions.Parse(serverConfig);

        var lstKeys = new List<string>();

        try
        {
            ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(options);

            var server = connection.GetServer(serverConfig.Split(":")[0], Convert.ToInt16(serverConfig.Split(":")[1]));

            lstKeys.AddRange(server.Keys(pattern: keyScan).Select(key => key.ToString()));

        }
        catch
        {
            throw new DisastersException.NotFound("get all cache not fount");
        }

        return lstKeys;
    }

}
