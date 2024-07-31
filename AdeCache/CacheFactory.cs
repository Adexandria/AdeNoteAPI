using AdeCache.Models;
using AdeCache.Services;
using AdeCache.Services.Exceptions;
using System.Reflection;

namespace AdeCache
{
    public class CacheFactory
    {
        public CacheFactory(Assembly assembly = null)
        {
           if(assembly == null)
           {
                cacheTypes = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(type => !type.IsAbstract && type.BaseType == typeof(CacheService)).Reverse().ToList();
           }
           else
           {
                cacheTypes = assembly.GetTypes()
                    .Where(type => !type.IsAbstract && type.BaseType == typeof(CacheService)).Reverse().ToList();
            }
        }

        public ICacheService CreateService(ICache cache)
        {
            ICacheService cacheService = null;

            var cacheType = cacheTypes.Where(s =>
            {
                cacheService = Activator.CreateInstance(s,cache) as ICacheService;
                return cacheService.CanConnect();
            }).FirstOrDefault();

            return cacheType == null ? throw new CacheException("Failed to create cache service") : cacheService;
        }

        private readonly List<Type> cacheTypes;
    }
}
