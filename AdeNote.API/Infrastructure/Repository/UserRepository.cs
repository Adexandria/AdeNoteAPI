using AdeNote.Db;
using Microsoft.EntityFrameworkCore;
using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    public class UserRepository : Repository,IUserRepository
    {
        public UserRepository(NoteDbContext db) : base(db)
        {

        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await Db.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<User> GetUserbyId(Guid userId)
        {
            return await Db.Users.AsNoTracking().FirstOrDefaultAsync(s=>s.Id == userId);
        }
    }
}
