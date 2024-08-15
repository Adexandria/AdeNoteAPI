using AdeCache.Models;
using AdeCache.Services.Utilities;
using NRedisStack;
using NRedisStack.RedisStackCommands;
using NRedisStack.Search;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;
using System.Text.Json;

namespace AdeCache.Services
{
    /// <summary>
    /// Manages cache services
    /// </summary>
    public class RedisCacheService : CacheService
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="cache">Manages caching configuration</param>
        public RedisCacheService(ICache cache)
        {
            _host = cache.HostName;
        }

        /// <summary>
        /// Get value using key
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Key</param>
        /// <returns>Value of the key</returns>
        public override T Get<T>(string key)
        {
            var value = _json.Get<T>(key);
            if(value == null)
            {
               return default;
            }
            return value;
        }

        /// <summary>
        /// Searches cache using key and parameter
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="parameter">Parameter used to search</param>
        /// <returns>A list of values</returns>
        public override IEnumerable<T> Search<T>(string key, string parameter)
        {
            var _searchCommand = _database.FT();

            var index = _database.StringGet(key).ToString();

            if (string.IsNullOrEmpty(index))
            {
                var properties = typeof(T).GetProperties().Select(s =>
               new PropertyInformation()
               {
                   Name = s.Name,
                   PropertyType = s.PropertyType
               }).ToList();

                var schema = new Schema();

                CreateSchema(schema, properties);
                _searchCommand.Create($"idx:{key}",
                    new FTCreateParams().On(IndexDataType.JSON).Prefix($"{key}:"), schema);

                _database.StringSet(key, $"idx:{key}");
            }
            
            var query = new Query(parameter);

            var value = _searchCommand.Search($"idx:{key}", query);

            if (value.TotalResults == 0)
            {
                return default;
            }

            var json = string.Join(",", value.Documents.Select(x => x["json"]));

            var arrayBracket = "[]";

            var jsonArray = arrayBracket.Insert(1,json);

            var result = JsonSerializer.Deserialize<List<T>>(jsonArray);

            return result;
        }

        /// <summary>
        /// Remove value using key
        /// </summary>
        /// <param name="key">Key</param>
        public override void Remove(string key)
        {
           _database.KeyDelete(key);
        }

        /// <summary>
        /// Sets value to a key
        /// </summary>
        /// <typeparam name="T">Type of the value</typeparam>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expiryDate">Expiration date of the cache</param>
        public override void Set<T>(string key, T value, DateTime expiryDate = default)
        {
            if(expiryDate == default)
            {
                _json.Set(key, "$", value);
                return;
            }

            _json.Set(key, "$", value);
            _database.KeyExpire(key, expiryDate);
        }

        /// <summary>
        /// Verifies if it can connect to the cache service
        /// </summary>
        /// <returns>Boolean value</returns>
        public override bool CanConnect()
        {
            try
            {
                var redis = ConnectionMultiplexer.Connect(_host);

                if (redis.IsConnected)
                {
                    _database = redis.GetDatabase();
                    _json = _database.JSON();
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Creates schema using for index seraching and querying
        /// </summary>
        /// <param name="schema">Existing schema of the model</param>
        /// <param name="properties">Properties of the model</param>
        /// <param name="left">pointer</param>
        private void CreateSchema(Schema schema, List<PropertyInformation> properties, int left = 0)
        {
            if(left >= properties.Count)
            {
                return;
            }

            if (IsNumericType(properties[left].PropertyType))
            {
                schema.AddNumericField(properties[left].Name);
            }
            else if (properties[left].PropertyType == typeof(string))
            {
               schema.AddTextField(properties[left].Name);
            }

            CreateSchema(schema, properties, left+1);
        }

        /// <summary>
        /// Verifies if it is a numeric type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>boolean value</returns>
        public static bool IsNumericType(Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32
                or TypeCode.UInt64 or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 or
                TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
                _ => false,
            };
        }

        private readonly string _host;
        private IDatabase _database;
        private JsonCommands _json;
    }
}
