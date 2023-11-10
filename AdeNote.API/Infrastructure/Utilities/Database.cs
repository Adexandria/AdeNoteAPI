using AdeNote.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using TasksLibrary.Architecture.Application;

namespace AdeNote.Infrastructure.Utilities
{
    public static class Database
    {
        public static void CreateTables(this IServiceProvider provider, TaskContainerBuilder containerBuilder)
        {
            var dbContext = provider.GetService<NoteDbContext>() ?? throw new NullReferenceException("Unregistered service");

            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (databaseCreator.HasTables())
            {
                containerBuilder.BuildMigration();
                databaseCreator.CreateTables();
            }
        }
    }
}
