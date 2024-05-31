using AdeAuth.Services;
using AdeNote.Db;
using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AdeNote.Infrastructure.Utilities
{
    public static class Database
    {
        public static void CreateTables(this IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            var dbContext =provider.GetService<NoteDbContext>()
                ?? throw new NullReferenceException("Unregistered service");

            var loggerFactory = provider.GetRequiredService<ILoggerFactory>() 
                ?? throw new NullReferenceException("Unregistered service");

            var logger = loggerFactory.CreateLogger<NoteDbContext>();

            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (!databaseCreator.HasTables())
            {
                logger.LogInformation("Created Tables successfully");
                databaseCreator.CreateTables();
            }
            logger.LogInformation("Tables have been created");
        }

        public static void SeedHangFireUser(this IServiceCollection services, HangFireUserConfiguration config)
        {
            if(config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var serviceProvider = services.BuildServiceProvider();
            var hangfireUserRepository = serviceProvider.GetService<IHangfireUserRepository>() 
                ?? throw new NullReferenceException("Unregistered service");

            var passwordManager = serviceProvider.GetService<IPasswordManager>() 
                ?? throw new NullReferenceException("Unregistered service");


            if (!hangfireUserRepository.IsSeeded)
            {
                var hangfireUser = new HangfireUser(config.Username);

                var hashedPassword = passwordManager.HashPassword(config.Password, out string salt);

                hangfireUser.SetPassword(hashedPassword,salt);

                hangfireUserRepository.Add(hangfireUser);
            }
        }

        public static void SeedSuperAdmin(this IServiceCollection services, DefaultConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var serviceProvider = services.BuildServiceProvider();

            var passwordManager = serviceProvider.GetService<IPasswordManager>()
                ?? throw new NullReferenceException("Unregistered service");

            var userRepository = serviceProvider.GetService<IUserRepository>()
               ?? throw new NullReferenceException("Unregistered service");

            if(!userRepository.IsExist(config.Email))
            { 
                var user = new User(config.FirstName, config.LastName, config.Email, AuthType.local, Role.SuperAdmin);

                user.ConfirmEmailVerification();

                var hashedPassword = passwordManager.HashPassword(config.Password, out string salt);

                user.SetPassword(hashedPassword,salt);

                userRepository.Add(user);
            }
        }
    }
}
