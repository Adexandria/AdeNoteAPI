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
            var dbContext = services.BuildServiceProvider().GetService<NoteDbContext>()
                ?? throw new NullReferenceException("Unregistered service");

            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (!databaseCreator.HasTables())
            {
                databaseCreator.CreateTables();
            }
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
    }
}
