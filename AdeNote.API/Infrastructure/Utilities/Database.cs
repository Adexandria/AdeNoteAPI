using AdeNote.Db;

namespace AdeNote.Infrastructure.Utilities
{
    public static class Database
    {
        public static void CreateTables(this IServiceProvider provider)
        {
            var dbContext = provider.GetService<NoteDbContext>() ?? throw new NullReferenceException("Unregistered service");

            dbContext.Database.EnsureCreated();
        }
    }
}
