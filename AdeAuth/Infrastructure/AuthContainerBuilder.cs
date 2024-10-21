using AdeAuth.Db;
using AdeAuth.Models;
using AdeAuth.Services;
using AdeAuth.Services.Interfaces;
using AdeAuth.Services.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Security.Claims;
using System.Text;

namespace AdeAuth.Infrastructure
{
    public static class AuthContainerBuilder
    {
        /// <summary>
        /// Sets up identity services using identity context
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="serviceCollection">Manages dependencies of services</param>
        /// <param name="actionBuilder">Registers db context dependencies</param>
        public static IServiceCollection AddIdentityService<TDbContext>(this IServiceCollection serviceCollection,
            Action<AuthConfiguration> configurationBuilder)
            where TDbContext : IdentityContext
        {
            var authConfiguration = new AuthConfiguration();

            configurationBuilder(authConfiguration);

            RegisterDbContext<TDbContext>(serviceCollection, authConfiguration.ConnectionString);

            if(authConfiguration.DependencyTypes.Any())
            {
               
                RegisterServices<TDbContext>(serviceCollection, authConfiguration.DependencyTypes);
            }
            else
            {
                RegisterDependencies<TDbContext>(serviceCollection);
            }
            return serviceCollection;
        }

        /// <summary>
        /// Sets up identity services using identity context
        /// </summary>
        /// <typeparam name="TUser">User model</typeparam>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="serviceCollection">Manages dependencies of services</param>
        /// <param name="actionBuilder">Registers db context dependencies</param>
        public static IServiceCollection AddIdentityService<TDbContext,TUser>(this IServiceCollection serviceCollection, 
            Action<AuthConfiguration> configurationBuilder = null) 
            where TDbContext : IdentityContext<TUser>
            where TUser : ApplicationUser, new()
        {
            var authConfiguration = new AuthConfiguration();

            configurationBuilder(authConfiguration);

            RegisterDbContext<TDbContext>(serviceCollection, authConfiguration.ConnectionString);

            if (!authConfiguration.DependencyTypes.Any())
            {
                RegisterDependencies<TDbContext,TUser>(serviceCollection);
            }
            else
            {
                RegisterServices<TDbContext,TUser>(serviceCollection, authConfiguration.DependencyTypes);
            }

            return serviceCollection;
        }

        /// <summary>
        /// Sets up identity services using identity context
        /// </summary>
        /// <typeparam name="TRole">Role model</typeparam>
        /// <typeparam name="TUser">User model</typeparam>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="serviceCollection">Manages dependencies of services</param>
        public static IServiceCollection AddIdentityService<TDbContext, TUser, TRole>(this IServiceCollection serviceCollection,
            Action<AuthConfiguration> configurationBuilder = null)
         where TDbContext : IdentityContext<TUser,TRole>
         where TUser : ApplicationUser, new()
         where TRole: ApplicationRole, new()
        {
            var authConfiguration = new AuthConfiguration();

            configurationBuilder(authConfiguration);

            RegisterDbContext<TDbContext>(serviceCollection, authConfiguration.ConnectionString);

            if (!authConfiguration.DependencyTypes.Any())
            {
                RegisterDependencies<TDbContext, TUser, TRole>(serviceCollection);
            }
            else
            {
                RegisterServices<TDbContext, TUser, TRole>(serviceCollection, authConfiguration.DependencyTypes);
            }
            return serviceCollection;
        }

        public static AuthenticationBuilder AddJwtBearer(this AuthenticationBuilder authenticationBuilder, Action<TokenConfiguration> actionBuilder)
        {
            var tokenConfiguration = new TokenConfiguration();

            actionBuilder(tokenConfiguration);

            authenticationBuilder
                .AddJwtBearer(tokenConfiguration.AuthenticationScheme ?? JwtBearerDefaults.AuthenticationScheme, option =>
                {
                    option.SaveToken = true;
                    option.TokenValidationParameters = new TokenValidationParameters() {

                        ValidAudience = tokenConfiguration.Audience ?? null,
                        ValidIssuer = tokenConfiguration.Issuer ?? null,
                        ValidateIssuer = !string.IsNullOrEmpty(tokenConfiguration.Issuer),
                        ValidateAudience = !string.IsNullOrEmpty(tokenConfiguration.Audience),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenConfiguration.TokenSecret)),
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true
                    };
                });

           authenticationBuilder.Services.AddSingleton(tokenConfiguration);

            return authenticationBuilder;
        }

        public static AuthenticationBuilder AddMicrosoftAccount(this AuthenticationBuilder authenticationBuilder, Action<AzureConfiguration> actionBuilder)
        {
           var azureConfiguration = new AzureConfiguration();

           actionBuilder(azureConfiguration);

            authenticationBuilder
                 .AddJwtBearer(azureConfiguration.AuthenticationScheme ?? JwtBearerDefaults.AuthenticationScheme, options =>
                 {
                     options.SaveToken = true;
                     options.MetadataAddress = $"{azureConfiguration.Instance}{azureConfiguration.Type}/v2.0/.well-known/openid-configuration";
                     options.TokenValidationParameters = new TokenValidationParameters()
                     {
                         NameClaimType = azureConfiguration.NameClaimType ?? ClaimsIdentity.DefaultNameClaimType,
                         ValidAudience = azureConfiguration.Audience,
                         ValidIssuer = $"{azureConfiguration.Instance}{azureConfiguration.TenantId}/v2.0",
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateIssuerSigningKey = true,
                         ValidateLifetime = true
                     };
                 });

            return authenticationBuilder;
        }


        /// <summary>
        /// Creates table if they don't exist
        /// </summary>
        /// <param name="dbContext">Db context</param>
        /// <param name="entityName">Entity name</param>
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

        /// <summary>
        /// Registers dbcontext
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        /// <param name="connectionString">Connection string of sql server</param>
        private static void RegisterDbContext<TDbContext>(IServiceCollection services, string connectionString)
            where TDbContext: DbContext
        {
            services.AddDbContext<TDbContext>((s) => s.UseSqlServer(connectionString));

            var provider = services.BuildServiceProvider();

            var scope = provider.CreateScope();

            RunMigration(scope.ServiceProvider.GetService<TDbContext>());

            RegisterDependencies(services);
        }

        /// <summary>
        /// Run migrations
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        /// <param name="action">Registers db context dependencies</param>
        private static void RunMigration<TDbContext>(TDbContext dbContext) 
            where TDbContext : DbContext
        {
            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (databaseCreator.CanConnect())
            {
                CheckTableExistsAndCreateIfMissing(dbContext, "Users");
                CheckTableExistsAndCreateIfMissing(dbContext, "Roles");
                CheckTableExistsAndCreateIfMissing(dbContext, "UserRoles");
            }
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        private static void RegisterDependencies<TDbContext>(IServiceCollection services)
           where TDbContext : IdentityContext
        {
            services.AddScoped<IRoleService<ApplicationRole>, RoleService<TDbContext, ApplicationUser, ApplicationRole>>();

            services.AddScoped<IUserService<ApplicationUser>, UserService<TDbContext, ApplicationUser>>();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <typeparam name="TUser">User model</typeparam>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        private static void RegisterDependencies<TDbContext,TUser>(IServiceCollection services)
            where TDbContext : IdentityContext<TUser>
            where TUser : ApplicationUser,new()
        {
            services.AddScoped<IRoleService<ApplicationRole>, RoleService<TDbContext, TUser,ApplicationRole>>();

            services.AddScoped<IUserService<TUser>, UserService<TDbContext, TUser>>();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <typeparam name="TUser">User model</typeparam>
        /// <typeparam name="TRole">Role model</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        private static void RegisterDependencies<TDbContext, TUser, TRole>(IServiceCollection services)
          where TDbContext : IdentityContext<TUser, TRole>
          where TRole : ApplicationRole, new()
           where TUser : ApplicationUser, new()
        {

            services.AddScoped<IRoleService<TRole>, RoleService<TDbContext, TUser,TRole>>();

            services.AddScoped<IUserService<TUser>, UserService<TDbContext, TUser>>();
        }

        /// <summary>
        /// Register dependencies
        /// </summary>
        /// <param name="services">Manages dependencies of services</param>
        private static void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton<IPasswordManager, PasswordManager>();

            services.AddSingleton<ITokenProvider,TokenProvider>();

            services.AddSingleton<IMfaService, MfaService>();   
        }

        /// <summary>
        /// Register services 
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        /// <param name="dependencyTypes">Types to register</param>
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

        /// <summary>
        /// Register services
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <typeparam name="TUser">User model</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        /// <param name="dependencyTypes">Types to register</param>
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

        /// <summary>
        /// Register services
        /// </summary>
        /// <typeparam name="TDbContext">Identity context</typeparam>
        /// <typeparam name="TUser">User model</typeparam>
        /// <typeparam name="TRole">Role model</typeparam>
        /// <param name="services">Manages dependencies of services</param>
        /// <param name="dependencyTypes">Types to register</param>
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
