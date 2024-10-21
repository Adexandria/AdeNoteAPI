using AdeAuth.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AdeAuth.Services.Utility
{
    public class AuthConfiguration
    {
        public AuthConfiguration()
        {
            DependencyTypes = new List<Type>();
        }

        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            DependencyTypes = assembly.GetTypes()
                .Where(s => GetGenericArguments(s) && !s.IsAbstract).ToList();
        }

        public void RegisterUserService(Type type)
        {
            DependencyTypes.Add(type);
        }

        public void RegisterRoleServiceFromAssembly(Type type)
        {
            DependencyTypes.Add(type);
        }

        public void UseSqlServer(string connectionString)
        {
            ConnectionString = connectionString;
        }

        private bool GetGenericArguments(Type type)
        {
            if(type  == null) return false;

            var hasServices = type.BaseType?.GetGenericArguments()
                .Any(s => s.BaseType == typeof(ApplicationUser) || s.BaseType == typeof(ApplicationRole));

            return hasServices.GetValueOrDefault();
        }

        public string ConnectionString { get; set; }
        public List<Type> DependencyTypes { get; private set; }
    }
}
