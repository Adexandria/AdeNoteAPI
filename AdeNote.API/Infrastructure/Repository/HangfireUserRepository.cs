using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class HangfireUserRepository : Repository, IHangfireUserRepository
    {
        public HangfireUserRepository(NoteDbContext dbContext): base(dbContext)
        {
            
        }

        public bool IsSeeded
        {
            get
            {
                return Db.HangfireUsers.Any();
            }
        }


        public async Task<bool> Add(HangfireUser entity)
        {
            entity.Id = Guid.NewGuid();
            await Db.HangfireUsers.AddAsync(entity);
            return await SaveChanges<HangfireUser>();
        }

        public async Task<bool> Remove(HangfireUser entity)
        {
            Db.HangfireUsers.Remove(entity);
            return await SaveChanges<HangfireUser>();
        }

        public async Task<bool> Update(HangfireUser entity)
        {
            var currentUser = Db.HangfireUsers.Where(s => s.Id == entity.Id).FirstOrDefault();

            Db.Entry(currentUser).CurrentValues.SetValues(entity);

            Db.Entry(currentUser).State = EntityState.Modified;

            return await SaveChanges<HangfireUser>();
        }
    }
}
