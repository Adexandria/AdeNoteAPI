using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{

    public class UserDetailRepository : Repository,IUserDetailRepository
    {
        public UserDetailRepository(NoteDbContext db) : base(db)
        {

        }
        public async Task<bool> Add(UserDetail entity)
        {
           entity.Id = Guid.NewGuid();
           await Db.UserDetails.AddAsync(entity);
           return await SaveChanges<UserDetail>();
        }

        public async Task<UserDetail> GetUserDetail(Guid userId)
        {
            return await Db.UserDetails.
                AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);
        }

        public async Task<bool> Remove(UserDetail entity)
        {
            Db.UserDetails.Remove(entity);
            return await SaveChanges<UserDetail>();
        }

        public async Task<bool> Update(UserDetail entity)
        {
            var currentUserDetail = Db.UserDetails.Where(s=>s.UserId == entity.UserId).FirstOrDefault();

            Db.Entry(currentUserDetail).CurrentValues.SetValues(entity);

            Db.Entry(currentUserDetail).State = EntityState.Modified;

            return await SaveChanges<UserDetail>();
        }
    }
}
