﻿using AdeCache.Models;
using AdeCache.Services;
using AdeCache.Services.Exceptions;
using System.Reflection;

namespace AdeCache
{
    /// <summary>
    /// Create cache services
    /// </summary>
    public class CacheFactory
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="assembly">Assembly that include custom cache services</param>
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

        /// <summary>
        /// Create service based on cache configuration
        /// </summary>
        /// <param name="cache">Manages ccahe configuration</param>
        /// <returns></returns>
        /// <exception cref="CacheException">Throws if it fails to create service</exception>
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
