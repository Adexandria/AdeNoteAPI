using AdeCache.Models;
using Microsoft.Extensions.Caching.Memory;


namespace AdeCache.Services
{
    public class MemoryCacheService : CacheService
    {
        public MemoryCacheService(ICache cache)
        {
            _memoryCache = cache.MemoryCache;
            _memoryCache.Set("Connect", true);
        }

        public override T Get<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }
            return default;
        }

        public override void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        public override void Set<T>(string key, T value, DateTime expiryDate = default)
        {
            if(expiryDate == null)
            {
                _memoryCache.Set(key, value);
            }
            else
            {
                _memoryCache.Set(key, value, new DateTimeOffset(expiryDate));
            }
        }

        public override bool CanConnect()
        {
            return _memoryCache.Get<bool>("Connect");
        }

        private readonly IMemoryCache _memoryCache;
    }
}
