namespace AdeCache.Services
{
    /// <summary>
    /// Manages cache service
    /// </summary>
    public abstract class CacheService : ICacheService
    {
        /// <summary>
        /// Verifies if it can connect to the cache service
        /// </summary>
        /// <returns>Boolean value</returns>
        public abstract bool CanConnect();

        /// <summary>
        /// Get value using key
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value of the key</returns>
        public abstract T Get<T>(string key);

        /// <summary>
        /// Remove value using key
        /// </summary>
        /// <param name="key">Key</param>
        public abstract void Remove(string key);

        /// <summary>
        /// Searches cache using key and parameter
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="parameter">Parameter used to search</param>
        /// <returns>A list of values</returns>
        public abstract IEnumerable<T> Search<T>(string key, string parameter);

        /// <summary>
        /// Sets value to a key
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiryDate">Expiration date of the cache</param>
        public abstract void Set<T>(string key, T value, DateTime expiryDate = default);
    }
}
