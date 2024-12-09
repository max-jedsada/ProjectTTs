using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Project.Manager.Redis;

public interface IRedisCacheService
{
    string GetCachedData(string key);
    void SetCachedData(string key, string dataJson, TimeSpan cacheDuration);
    void Remove(string key);

    Task<List<string>> GetAll(string keyScan);

}
