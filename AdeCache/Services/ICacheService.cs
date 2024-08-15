namespace AdeCache.Services
{
    /// <summary>
    /// Manages cache services
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Remove value using key
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);

        /// <summary>
        /// Get value using key
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value of the key</returns>
        T Get<T>(string key);

        /// <summary>
        /// Searches cache using key and parameter
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="parameter">Parameter used to search</param>
        /// <returns>A list of values</returns>
        IEnumerable<T> Search<T>(string key, string parameter = null);

        /// <summary>
        /// Sets value to a key
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiryDate">Expiration date of the cache</param>
        void Set<T>(string key, T value, DateTime expiryDate = default);

        /// <summary>
        /// Verifies if it can connect to the cache service
        /// </summary>
        /// <returns>Boolean value</returns>
        bool CanConnect();
    }
}
