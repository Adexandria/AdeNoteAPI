namespace AdeCache.Services
{
    public interface ICacheService
    {
        void Remove(string key);
        T Get<T>(string key);
        void Set<T>(string key, T value, DateTime expiryDate = default);
        bool CanConnect();
    }
}
