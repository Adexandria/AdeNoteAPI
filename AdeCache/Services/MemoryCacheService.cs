using AdeCache.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Reflection;


namespace AdeCache.Services
{
    /// <summary>
    /// Manages caching service
    /// </summary>
    public class MemoryCacheService : CacheService
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="cache">Manages caching configuration</param>
        public MemoryCacheService(ICache cache)
        {
            _memoryCache = cache.MemoryCache;
            _memoryCache.Set("Connect", true);
        }

        /// <summary>
        /// Get value using key
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value of the key</returns>
        public override T Get<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out T cachedValue))
            {
                return cachedValue;
            }
            return default;
        }

        /// <summary>
        /// Remove value using key
        /// </summary>
        /// <param name="key">Key</param>
        public override void Remove(string key)
        {
            _memoryCache.Remove(key);
        }

        /// <summary>
        /// Sets value to a key
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiryDate">Expiration date of the cache</param>
        public override void Set<T>(string key, T value, DateTime expiryDate)
        {
            if(expiryDate == default)
            {
                _memoryCache.Set(key, value);
            }
            else
            {
                _memoryCache.Set(key, value, new DateTimeOffset(expiryDate));
            }
        }

        /// <summary>
        /// Verifies if it can connect to the cache service
        /// </summary>
        /// <returns>Boolean value</returns>
        public override bool CanConnect()
        {
            return _memoryCache.Get<bool>("Connect");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        /// 
        // make use of the parameter variable
        public override IEnumerable<T> Search<T>(string key, string parameter)
        {
            var coherentState = typeof(MemoryCache).GetField("_coherentState", BindingFlags.NonPublic | BindingFlags.Instance);

            var coherentStateValue = coherentState.GetValue(_memoryCache);

            var entriesCollection = coherentStateValue.GetType().GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

            var result = new List<T>();

            if (entriesCollection.GetValue(coherentStateValue) is ICollection entriesCollectionValue)
            {
                foreach (var item in entriesCollectionValue)
                {
                    var methodInfo = item.GetType().GetProperty("Key");

                    var val = methodInfo.GetValue(item).ToString();

                    if (val.StartsWith(key))
                    {
                        result.Add(_memoryCache.Get<T>(val));
                    }
                }
                return result.Count == 0 ? default : result;
            }

            return default;
        }

        private readonly IMemoryCache _memoryCache;
    }
}
