using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace RedisCachingDemo.Services
{
    public class CacheService
    {
        private readonly IDatabase _cacheDb;

        public CacheService(IConnectionMultiplexer redis)
        {
            _cacheDb = redis.GetDatabase();
        }

        public async Task<string> GetCachedValueAsync(string key)
        {
            return await _cacheDb.StringGetAsync(key);
        }

        public async Task SetCacheValueAsync(string key, string value)
        {
            await _cacheDb.StringSetAsync(key, value, TimeSpan.FromMinutes(5)); // cache expiry of 5 mins
        }
    }
}
