using Microsoft.Extensions.Caching.Memory;


namespace AdeCache.Models
{
    /// <summary>
    /// Manage caching configuration
    /// </summary>
    public interface ICache
    {
        /// <summary>
        /// Handles memoryCache
        /// </summary>
        IMemoryCache MemoryCache { get; set; }

        /// <summary>
        /// Host name of redis cache
        /// </summary>
        string HostName {  get; set; }
    }
}
