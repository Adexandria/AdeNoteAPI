using AdeCache.Models;
using Microsoft.Extensions.Caching.Memory;

namespace AdeNote.Infrastructure.Utilities.CacheModel
{
    public class Cache : ICache
    {
        public Cache(string hostName, IMemoryCache memoryCache)
        {
            MemoryCache = memoryCache;
            HostName = hostName;
        }
        public IMemoryCache MemoryCache { get; set; }
        public string HostName { get; set; }
    }
}
