using AdeAuth.Db;
using AdeAuth.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AdeAuth.Services.Utility
{
    public class AuthConfiguration
    {
        public AuthConfiguration()
        {
            builder = new DbContextOptionsBuilder();
            DependencyTypes = new List<Type>();
        }

        public void RegisterServicesFromAssembly(Assembly assembly)
        {
            var type = assembly.GetTypes()
                .Where(s => s.BaseType.IsGenericType && s.BaseType.GetGenericArguments()
                .Any(p => p == typeof(ApplicationUser))).ToList();

            DependencyTypes = type;
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
            ActionBuilder += (_) => builder.UseSqlServer(connectionString);
        }

        private readonly DbContextOptionsBuilder builder;
        public Action<DbContextOptionsBuilder> ActionBuilder { get; set; }
        public List<Type> DependencyTypes { get; private set; }
    }
}
