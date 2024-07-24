using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Interfaces;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace AdeAuth.Infrastructure
{
    public class AuthContainerBuilder
    {
        internal void RegisterDependencies()
        {
            ContainerBuilder = new ContainerBuilder();
            ContainerBuilder.RegisterType<RoleService>()
                .As<IRoleService>().
                PropertiesAutowired();

            ContainerBuilder.RegisterType<UserService>()
                .As<IUserService<IApplicationUser>>()
                .PropertiesAutowired();
        }

        public static void UseIdentityService<TDbContext>(Action<DbContextOptionsBuilder> dbOptionBuilder) where TDbContext : DbContext
        {
            var dbContextType = typeof(TDbContext);
            var target = dbOptionBuilder.Target as DbContextOptionsBuilder;
            TDbContext dbContext = Activator.CreateInstance(dbContextType,dbOptionBuilder.i);

            dbContext.
            ContainerBuilder.RegisterType<TDbContext>()
                .SingleInstance();
        }

        private static ContainerBuilder ContainerBuilder;
    }
}
