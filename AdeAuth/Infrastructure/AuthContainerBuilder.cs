using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Interfaces;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;


namespace AdeAuth.Infrastructure
{
    public class AuthContainerBuilder
    {
        internal static void RegisterDependencies<TDbContext>() where TDbContext : DbContext
        {
            
            ContainerBuilder.RegisterType<RoleService<TDbContext>>()
                .As<IRoleService>().
                PropertiesAutowired();

            ContainerBuilder.RegisterType<UserService>()
                .As<IUserService<IApplicationUser>>()
                .PropertiesAutowired();
           
        }

        public static void UseIdentityService<TDbContext>(Action<DbContextOptionsBuilder> dbOptionBuilder) where TDbContext : DbContext
        {
            ContainerBuilder = new ContainerBuilder();

            var dbContextBuilder = new DbContextOptionsBuilder<TDbContext>();

            dbOptionBuilder(dbContextBuilder);

            var dbContext = Activator.CreateInstance(typeof(TDbContext), dbContextBuilder.Options) as TDbContext;
           
            ContainerBuilder.Register((_) => dbContext)
                .As<TDbContext>()
                .SingleInstance();

            RunMigration<TDbContext>(dbContext);

            RegisterDependencies<TDbContext>();
        }


        private static void RunMigration<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (databaseCreator.CanConnect())
            {
                dbContext.Database.Migrate();
            }
        }

        private static ContainerBuilder ContainerBuilder;
    }
}
