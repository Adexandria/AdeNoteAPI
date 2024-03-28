using AdeNote.Db;
using Microsoft.EntityFrameworkCore;
using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    public class UserRepository : Repository, IUser
    {
        public UserRepository(NoteDbContext dbContext) : base(dbContext)
        {

        }

        public bool IsExist(string email)
        {
            return Db.Users.Any(s => s.Email == email);
        }

        public async Task<bool> UpdateUser(User currentUser)
        {
            Db.Users.Update(currentUser);
            return await SaveChanges<User>();
        }
    }
}
