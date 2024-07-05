namespace AdeCache.Services
{
    public abstract class CacheService : ICacheService
    {
        public abstract bool CanConnect();
        public abstract T Get<T>(string key);

        public abstract void Remove(string key);

        public abstract void Set<T>(string key, T value, DateTime expiryDate = default);
    }
}
