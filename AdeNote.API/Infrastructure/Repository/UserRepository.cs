using AdeNote.Db;
using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    public class UserRepository : Repository, IUser
    {
        public UserRepository(NoteDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> UpdateUser(User currentUser)
        {
            Db.Users.Update(currentUser);
            return await SaveChanges<User>();
        }
    }
}
