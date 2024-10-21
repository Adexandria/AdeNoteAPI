using AdeAuth.Models;
using System.Reflection;

namespace AdeAuth.Services.Utility
{
    /// <summary>
    /// Builds configuration for authentication library
    /// </summary>
    public class AuthConfiguration
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public AuthConfiguration()
        {
            DependencyTypes = new List<Type>();
        }

        /// <summary>
        /// Register dependencies using assembly searching
        /// </summary>
        /// <param name="assembly"></param>
        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            DependencyTypes = assembly.GetTypes()
                .Where(s => GetGenericArguments(s) && !s.IsAbstract).ToList();
        }

        /// <summary>
        /// Register only user services using type
        /// </summary>
        /// <param name="type"></param>
        public void RegisterUserService(Type type)
        {
            DependencyTypes.Add(type);
        }

        /// <summary>
        /// Register only role service using it's type
        /// </summary>
        /// <param name="type"></param>
        public void RegisterRoleServiceFromAssembly(Type type)
        {
            DependencyTypes.Add(type);
        }

        /// <summary>
        /// Sets up sql server
        /// </summary>
        /// <param name="connectionString"></param>
        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Searches for the type
        /// </summary>
        /// <param name="type">Type to register</param>
        /// <returns>boolean</returns>
        private bool GetGenericArguments(Type type)
        {
            if(type  == null) return false;

            var hasServices = type.BaseType?.GetGenericArguments()
                .Any(s => s.BaseType == typeof(ApplicationUser) || s.BaseType == typeof(ApplicationRole));

            return hasServices.GetValueOrDefault();
        }

        /// <summary>
        /// Connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// List of dependencies to register
        /// </summary>
        public List<Type> DependencyTypes { get; private set; }
    }
}
