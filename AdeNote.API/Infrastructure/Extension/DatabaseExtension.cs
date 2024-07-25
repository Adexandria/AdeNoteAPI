using AdeAuth.Services.Interfaces;
using AdeNote.Db;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.AuthenticationFilter;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AdeNote.Infrastructure.Extension
{
    public static class DatabaseExtension
    {
        public static void CreateTables(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var provider = scope.ServiceProvider;

            var dbContext = provider.GetService<NoteDbContext>()
                ?? throw new NullReferenceException("Unregistered service");

            var loggerFactory = provider.GetRequiredService<ILoggerFactory>()
                ?? throw new NullReferenceException("Unregistered service");

            var logger = loggerFactory.CreateLogger<NoteDbContext>();

            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (databaseCreator.CanConnect())
            {
                dbContext.Database.Migrate();
            }

            logger.LogInformation("Tables have been created");
        }

        public static void SeedHangFireUser(this WebApplication app, HangFireUserConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            using var scope = app.Services.CreateScope();

            var serviceProvider = scope.ServiceProvider;

            var hangfireUserRepository = serviceProvider.GetService<IHangfireUserRepository>()
                ?? throw new NullReferenceException("Unregistered service");

            var passwordManager = serviceProvider.GetService<IPasswordManager>()
                ?? throw new NullReferenceException("Unregistered service");

            if (!hangfireUserRepository.IsSeeded)
            {
                var hangfireUser = new HangfireUser(config.Username);

                var hashedPassword = passwordManager.HashPassword(config.Password, out string salt);

                hangfireUser.SetPassword(hashedPassword, salt);

                hangfireUserRepository.Add(hangfireUser);
            }
        }

        public static void SeedSuperAdmin(this WebApplication app, DefaultConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            using var scope = app.Services.CreateScope();

            var serviceProvider = scope.ServiceProvider;

            var passwordManager = serviceProvider.GetService<IPasswordManager>()
                ?? throw new NullReferenceException("Unregistered service");

            var userRepository = serviceProvider.GetService<IUserRepository>()
               ?? throw new NullReferenceException("Unregistered service");

            if (!userRepository.IsExist(config.Email))
            {
                var user = new User(config.FirstName, config.LastName, config.Email, AuthType.local, Role.SuperAdmin);

                user.ConfirmEmailVerification();

                var hashedPassword = passwordManager.HashPassword(config.Password, out string salt);

                user.SetPassword(hashedPassword, salt);

                userRepository.Add(user);
            }
        }
    }
}
