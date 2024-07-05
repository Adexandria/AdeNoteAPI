using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class HangfireUserRepository : Repository<HangfireUser>, IHangfireUserRepository
    {
        public HangfireUserRepository(NoteDbContext dbContext, ILoggerFactory loggerFactory): base(dbContext,loggerFactory)
        {
        }

        public bool IsSeeded
        {
            get
            {
                return Db.HangfireUsers.Any();
            }
        }

        public IList<HangfireUser> hangfireUsers => Db.HangfireUsers.ToList();

        public async Task<bool> Add(HangfireUser entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.HangfireUsers.AddAsync(entity);

            var result =  await SaveChanges();

            logger.LogInformation("Add hang fire user to database : {result}", result);

            return result;
        }

        public async Task<bool> Remove(HangfireUser entity)
        {
            Db.HangfireUsers.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Remove hang fire user to database : {result}", result);

            return result;
        }

        public async Task<bool> Update(HangfireUser entity)
        {
            var currentUser = Db.HangfireUsers.Where(s => s.Id == entity.Id).FirstOrDefault();

            Db.Entry(currentUser).CurrentValues.SetValues(entity);

            Db.Entry(currentUser).State = EntityState.Modified;

            var result = await SaveChanges();

            logger.LogInformation("Update hang fire user to database : {result}", result);

            return result;
        }
    }
}
