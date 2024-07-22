using AdeCache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AdeNote.Infrastructure.Utilities.CacheModel
{
    public class Cache : ICache
    {
        public Cache(string hostName)
        {
            HostName = hostName;
        }

        public Cache SetMemoryCache(IMemoryCache memoryCache) 
        { 
            MemoryCache = memoryCache;
            return this;
        }

        public IMemoryCache MemoryCache { get; set; }
        public string HostName { get; set; }
    }
}
