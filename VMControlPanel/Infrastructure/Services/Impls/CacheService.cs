using Infrastructure.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Infrastructure.Services.Impls
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _redis;

        public CacheService(IDistributedCache redis)
        {
            _redis = redis;
        }

        public async Task<T?> GetValueAsync<T>(string key)
        {
            var stringData = await _redis.GetStringAsync(key);

            return !string.IsNullOrEmpty(stringData) ? JsonConvert.DeserializeObject<T>(stringData) : default; 
        }

        public async Task SetValueAsync<T>(string key, T value, float expTimeInHours)
        {
            var stringData = JsonConvert.SerializeObject(value);
            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(expTimeInHours))
                .SetAbsoluteExpiration(DateTime.Now.AddHours(expTimeInHours));

            await _redis.SetStringAsync(key, stringData, options);
        }

        public async Task RemoveDataAsync(string key)
        {
            await _redis.RemoveAsync(key);
        }
    }
}
