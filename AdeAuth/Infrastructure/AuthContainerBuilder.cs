using AdeAuth.Db;
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
    public static class AuthContainerBuilder
    {
        public static void UseIdentityService<TDbContext>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> actionBuilder) 
            where TDbContext : IdentityContext
        {
            var containerBuilder = new ContainerBuilder();

            RegisterDbContext<TDbContext>(actionBuilder, containerBuilder);

            RegisterDependencies<TDbContext>(containerBuilder);

            var container = containerBuilder.Build();

            using var scope = container.BeginLifetimeScope();

            var application = scope.Resolve<Application<ApplicationUser, ApplicationRole>>();

            serviceCollection.RegisterServices(application);
        }

        public static void UseIdentityService<TDbContext,TUser>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> actionBuilder) 
            where TDbContext : IdentityContext
            where TUser : ApplicationUser, new()
        {
            var containerBuilder = new ContainerBuilder();

            RegisterDbContext<TDbContext>(actionBuilder, containerBuilder);

            RegisterDependencies<TDbContext,TUser>(containerBuilder);

            var container = containerBuilder.Build();

            using var scope = container.BeginLifetimeScope();

            var application = scope.Resolve<Application<TUser, ApplicationRole>>();

            serviceCollection.RegisterServices(application);
        }

        public static void UseIdentityService<TDbContext, TUser, TRole>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> actionBuilder)
         where TDbContext : IdentityContext
         where TUser : ApplicationUser, new()
         where TRole: ApplicationRole, new()
        {
            var containerBuilder = new ContainerBuilder();

            RegisterDbContext<TDbContext>(actionBuilder, containerBuilder);

            RegisterDependencies<TDbContext, TUser, TRole>(containerBuilder);

            var container = containerBuilder.Build();

            using var scope = container.BeginLifetimeScope();

            var application = scope.Resolve<Application<TUser, TRole>>();

            serviceCollection.RegisterServices(application);
        }

       private static void RunMigration<TDbContext>(TDbContext dbContext) where TDbContext : DbContext
        {
         
         var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

         if (databaseCreator.CanConnect())
         {
             CheckTableExistsAndCreateIfMissing(dbContext, "Users");
             CheckTableExistsAndCreateIfMissing(dbContext, "Roles");
         }

        }

        private static void CheckTableExistsAndCreateIfMissing(DbContext dbContext, string entityName)
        {
            var defaultSchema = "dbo";
            var tableName = string.IsNullOrWhiteSpace(defaultSchema) ? $"[{entityName}]" : $"[{defaultSchema}].[{entityName}]";

            try
            {
                _ = dbContext.Database.ExecuteSqlRaw($"SELECT TOP(1) * FROM {tableName}"); //Throws on missing table
            }
            catch (Exception)
            {
                var scriptStart = $"CREATE TABLE [{entityName}]";
                const string scriptEnd = "GO";
                var script = dbContext.Database.GenerateCreateScript();

                var tableScript = script.Split(scriptStart).Last().Split(scriptEnd);
                var first = $"{scriptStart} {tableScript.First()}";

                dbContext.Database.ExecuteSqlRaw(first);
            }
        }

        private static void RegisterDbContext<TDbContext>(Action<DbContextOptionsBuilder> actionBuilder, ContainerBuilder containerBuilder) where TDbContext: DbContext
        {
            var dbContextBuilder = new DbContextOptionsBuilder();

            actionBuilder(dbContextBuilder);

            var dbContext = Activator.CreateInstance(typeof(TDbContext), dbContextBuilder.Options) as TDbContext;

            containerBuilder.Register((_) => dbContext)
                .As<TDbContext>()
                .SingleInstance();

            RunMigration<TDbContext>(dbContext);

            RegisterDependencies(containerBuilder);
        }

        private static void RegisterDependencies<TDbContext>(ContainerBuilder containerBuilder) where TDbContext : DbContext
        {

            containerBuilder.RegisterType<RoleService<TDbContext,ApplicationRole>>()
                .As<IRoleService<ApplicationRole>>().
                PropertiesAutowired()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<UserService<TDbContext, ApplicationUser>>()
                .As<IUserService<ApplicationUser>>()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<Application<ApplicationUser, ApplicationRole>>()
              .As<Application<ApplicationUser, ApplicationRole>>()
              .PropertiesAutowired()
              .InstancePerLifetimeScope();
        }

        private static void RegisterDependencies<TDbContext,TUser>(ContainerBuilder containerBuilder)
            where TDbContext : DbContext
            where TUser : ApplicationUser,new()
        {

            containerBuilder.RegisterType<RoleService<TDbContext,ApplicationRole>>()
                .As<IRoleService<ApplicationRole>>().
                PropertiesAutowired()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<UserService<TDbContext,TUser>>()
                .As<IUserService<TUser>>()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<Application<TUser, ApplicationRole>>()
                .As<Application<TUser, ApplicationRole>>()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();
        }

        private static void RegisterDependencies<TDbContext, TUser, TRole>(ContainerBuilder containerBuilder)
          where TDbContext : IdentityContext
          where TUser : ApplicationUser, new()
          where TRole : ApplicationRole, new()
        {

            containerBuilder.RegisterType<RoleService<TDbContext, TRole>>()
                .As<IRoleService<TRole>>().
                PropertiesAutowired()
                .InstancePerLifetimeScope();
             

            containerBuilder.RegisterType<UserService<TDbContext, TUser>>()
                .As<IUserService<TUser>>()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<Application<TUser, TRole>>()
             .As<Application<TUser, TRole>>()
             .PropertiesAutowired()
             .InstancePerLifetimeScope();
        }

        private static void RegisterDependencies(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<PasswordManager>()
                .As<IPasswordManager>()
                .SingleInstance();

            containerBuilder.RegisterType<TokenProvider>()
                .As<ITokenProvider>()
                .SingleInstance();

            containerBuilder.RegisterType<MfaService>()
                .As<IMfaService>()
                .SingleInstance();
        }

        private static void RegisterServices<TUser, TRole>(this IServiceCollection serviceCollection, Application<TUser, TRole> application)
            where TUser : ApplicationUser
            where TRole : ApplicationRole
        {
            serviceCollection.AddSingleton(application.MfaService);
            serviceCollection.AddSingleton(application.PasswordManager);
            serviceCollection.AddSingleton(application.TokenProvider);
            serviceCollection.AddScoped(application.RoleService.GetType());
            serviceCollection.AddScoped(application.UserService.GetType());
        }
    }
}
