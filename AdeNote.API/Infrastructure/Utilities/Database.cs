using AdeNote.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace AdeNote.Infrastructure.Utilities
{
    public static class Database
    {
        public static void CreateTables(this IServiceProvider provider)
        {
            var dbContext = provider.GetService<NoteDbContext>() ?? throw new NullReferenceException("Unregistered service");

            var databaseCreator = dbContext.GetService<IRelationalDatabaseCreator>();

            if (!databaseCreator.HasTables())
            {
                databaseCreator.CreateTables();
            }
        }
    }
}
