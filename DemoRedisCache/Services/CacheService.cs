using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DemoRedisCache.Services
{
    public interface ICacheService
    {
        public Task<string?> GetAsync(string key);

        public Task SetAsync(string key, object value, TimeSpan timeOut);
    }

    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(
            IDistributedCache distributedCache
        )
        {
            _distributedCache = distributedCache;
        }

        public async Task<string?> GetAsync(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return null;
            }
            var cacheResponse = await _distributedCache.GetStringAsync(key);
            return string.IsNullOrEmpty(cacheResponse) ? null : cacheResponse;
        }

        public async Task SetAsync(string key, object value, TimeSpan timeOut)
        {
            if(string.IsNullOrWhiteSpace(key) || value is null)
            {
                return;
            }
            var serialize = JsonConvert.SerializeObject(value, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });

            await _distributedCache.SetStringAsync(
                key, serialize, 
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = timeOut
                }
            );
        }
    }
}
