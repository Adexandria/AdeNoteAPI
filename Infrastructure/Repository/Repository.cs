using AdeNote.Db;

namespace AdeNote.Infrastructure.Repository
{
    public class Repository
    {
        public Repository(NoteDbContext noteDb)
        {
            _db = noteDb;
        }
        protected readonly NoteDbContext _db;

        protected async Task<bool> SaveChanges()
        {
            int commitedResult = await _db.SaveChangesAsync();
            if (commitedResult == 0)
                return false;
            return true;
        }
    }
}
