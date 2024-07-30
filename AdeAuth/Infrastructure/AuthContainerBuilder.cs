using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;



namespace AdeAuth.Infrastructure
{
    public static class AuthContainerBuilder
    {

        public static void UseIdentityService<TDbContext>(this IServiceCollection serviceCollection,
            Action<DbContextOptionsBuilder> actionBuilder,
            Func<Assembly, List<Type>> dependencies = null, Assembly assembly = null)
            where TDbContext : IdentityContext
        {
            RegisterDbContext<TDbContext>(serviceCollection, actionBuilder);


            if(assembly != null)
            {
                var dependencyTypes = dependencies(assembly);

                RegisterServices<TDbContext>(serviceCollection, dependencyTypes);
            }
            else
            {
                RegisterDependencies<TDbContext>(serviceCollection);
            }
        }

        public static void UseIdentityService<TDbContext,TUser>(this IServiceCollection serviceCollection, 
            Action<DbContextOptionsBuilder> actionBuilder, Func<Assembly, List<Type>> dependencies = null, Assembly assembly = null) 
            where TDbContext : IdentityContext<TUser>
            where TUser : ApplicationUser, new()
        {
            RegisterDbContext<TDbContext>(serviceCollection, actionBuilder);

            if (assembly == null)
            {
                RegisterDependencies<TDbContext,TUser>(serviceCollection);
            }
            else
            {
                RegisterServices<TDbContext,TUser>(serviceCollection, dependencies(assembly));
            }

        }

        public static void UseIdentityService<TDbContext, TUser, TRole>(this IServiceCollection serviceCollection, Action<DbContextOptionsBuilder> actionBuilder,
            Func<Assembly, List<Type>> dependencies = null, Assembly assembly = null)
         where TDbContext : IdentityContext<TUser,TRole>
         where TUser : ApplicationUser, new()
         where TRole: ApplicationRole, new()
        {
            RegisterDbContext<TDbContext>(serviceCollection, actionBuilder);

            if (assembly == null)
            {
                RegisterDependencies<TDbContext, TUser, TRole>(serviceCollection);
            }
            else
            {
                RegisterServices<TDbContext, TUser, TRole>(serviceCollection, dependencies(assembly));
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

        private static void RegisterDbContext<TDbContext>(IServiceCollection services, Action<DbContextOptionsBuilder> actionBuilder)
            where TDbContext: DbContext
        {
            services.AddDbContext<TDbContext>(actionBuilder);

            RunMigration(services,(s) => s.GetRequiredService<TDbContext>());

            RegisterDependencies(services);
        }

        private static void RunMigration<TDbContext>(IServiceCollection services, Func<IServiceProvider,TDbContext> action) 
            where TDbContext : DbContext
        {
            TDbContext dbContext = action(services.BuildServiceProvider());

            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (databaseCreator.CanConnect())
            {
                CheckTableExistsAndCreateIfMissing(dbContext, "Users");
                CheckTableExistsAndCreateIfMissing(dbContext, "Roles");
                CheckTableExistsAndCreateIfMissing(dbContext, "UserRoles");
            }
        }

        private static void RegisterDependencies<TDbContext>(IServiceCollection services)
           where TDbContext : IdentityContext
        {
            services.AddScoped<IRoleService<ApplicationRole>, RoleService<TDbContext, ApplicationUser, ApplicationRole>>();

            services.AddScoped<IUserService<ApplicationUser>, UserService<TDbContext, ApplicationUser>>();
        }

        private static void RegisterDependencies<TDbContext,TUser>(IServiceCollection services)
            where TDbContext : IdentityContext<TUser>
            where TUser : ApplicationUser,new()
        {
            services.AddScoped<IRoleService<ApplicationRole>, RoleService<TDbContext, TUser,ApplicationRole>>();

            services.AddScoped<IUserService<TUser>, UserService<TDbContext, TUser>>();
        }

        private static void RegisterDependencies<TDbContext, TUser, TRole>(IServiceCollection services)
          where TDbContext : IdentityContext<TUser, TRole>
          where TRole : ApplicationRole, new()
           where TUser : ApplicationUser, new()
        {

            services.AddScoped<IRoleService<TRole>, RoleService<TDbContext, TUser,TRole>>();

            services.AddScoped<IUserService<TUser>, UserService<TDbContext, TUser>>();
        }

        private static void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton<IPasswordManager, PasswordManager>();

            services.AddSingleton<ITokenProvider,TokenProvider>();

            services.AddSingleton<IMfaService, MfaService>();   
        }

        private static void RegisterServices<TDbContext>(IServiceCollection services, List<Type> dependencyTypes)
            where TDbContext : IdentityContext
        {
            var roleServiceType = dependencyTypes.Where(s => s.GetInterfaces().Contains(typeof(IRoleService<ApplicationRole>))).FirstOrDefault();
            var userServiceType = dependencyTypes.Where(s => s.GetInterfaces().Contains(typeof(IUserService<ApplicationUser>))).FirstOrDefault();

            if(roleServiceType != null && userServiceType != null)
            {
                services.AddScoped(typeof(IUserService<ApplicationUser>), userServiceType);
                services.AddScoped(typeof(IRoleService<ApplicationRole>), roleServiceType);
            }
            else if(userServiceType != null)
            {
                services.AddScoped(typeof(IUserService<ApplicationUser>), userServiceType);
                services.AddScoped<IRoleService<ApplicationRole>, RoleService<TDbContext, ApplicationUser, ApplicationRole>>();
            }else
            {
                services.AddScoped(typeof(IRoleService<ApplicationRole>), roleServiceType);
                services.AddScoped<IUserService<ApplicationUser>, UserService<TDbContext, ApplicationUser>>();
            }
        }

        private static void RegisterServices<TDbContext,TUser>(IServiceCollection services, List<Type> dependencyTypes)
           where TDbContext : IdentityContext<TUser>
            where TUser : ApplicationUser,new()
        {
            var roleServiceType = dependencyTypes.Where(s => s.GetInterfaces().Contains(typeof(IRoleService<ApplicationRole>))).FirstOrDefault();
            var userServiceType = dependencyTypes.Where(s => s.GetInterfaces().Contains(typeof(IUserService<TUser>))).FirstOrDefault();

            if (roleServiceType != null && userServiceType != null)
            {
                services.AddScoped(typeof(IUserService<TUser>), userServiceType);
                services.AddScoped(typeof(IRoleService<ApplicationRole>), roleServiceType);
            }
            else if (userServiceType != null)
            {
                services.AddScoped(typeof(IUserService<TUser>), userServiceType);
                services.AddScoped<IRoleService<ApplicationRole>, RoleService<TDbContext, TUser, ApplicationRole>>();
            }
            else
            {
                services.AddScoped(typeof(IRoleService<ApplicationRole>), roleServiceType);
                services.AddScoped<IUserService<TUser>, UserService<TDbContext, TUser>>();
            }
        }

        private static void RegisterServices<TDbContext, TUser,TRole>(IServiceCollection services, List<Type> dependencyTypes)
           where TDbContext : IdentityContext<TUser,TRole>
            where TUser : ApplicationUser, new()
            where TRole: ApplicationRole,new()
        {
            var roleServiceType = dependencyTypes.Where(s => s.GetInterfaces().Contains(typeof(IRoleService<TRole>))).FirstOrDefault();
            var userServiceType = dependencyTypes.Where(s => s.GetInterfaces().Contains(typeof(IUserService<TUser>))).FirstOrDefault();

            if (roleServiceType != null && userServiceType != null)
            {
                services.AddScoped(typeof(IUserService<TUser>), userServiceType);
                services.AddScoped(typeof(IRoleService<TRole>), roleServiceType);
            }
            else if (userServiceType != null)
            {
                services.AddScoped(typeof(IUserService<TUser>), userServiceType);
                services.AddScoped<IRoleService<TRole>, RoleService<TDbContext, TUser, TRole>>();
            }
            else
            {
                services.AddScoped(typeof(IRoleService<TRole>), roleServiceType);
                services.AddScoped<IUserService<TUser>, UserService<TDbContext, TUser>>();
            }
        }
    }
}
