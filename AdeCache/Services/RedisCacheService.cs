using AdeCache.Models;
using NRedisStack.RedisStackCommands;
using StackExchange.Redis;

namespace AdeCache.Services
{
    public class RedisCacheService : CacheService
    {
        public RedisCacheService(ICache cache)
        {
            _host = cache.HostName;
        }
        public override T Get<T>(string key)
        {
            var value = _database.JSON().Get<T>(key);
            if(value == null)
            {
               return default;
            }
            return value;
        }

        public override void Remove(string key)
        {
           _database.KeyDelete(key);
        }

        public override void Set<T>(string key, T value, DateTime expiryDate = default)
        {
            if(expiryDate == default)
            {
                _database.JSON().Set(key, "$", value);
                return;
            }
            _database.JSON().Set(key, "$", value);
        }

        public override bool CanConnect()
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect(_host);

                if (redis.IsConnected)
                {
                    _database = redis.GetDatabase();
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private readonly string _host;
        private IDatabase _database;
    }
}
