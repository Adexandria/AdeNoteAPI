using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class AuthRepository : Repository, IAuthRepository
    {
        public AuthRepository(NoteDbContext db) : base(db)
        {
            
        }
        public async Task<bool> Add(UserToken entity)
        {
            entity.Id = Guid.NewGuid();
            await Db.UserTokens.AddAsync(entity);
            return await SaveChanges<UserToken>();
        }

        public async Task<string> GenerateMFALoginToken(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<UserToken> GetAuthenticationType(Guid userId)
        {
            return await Db.UserTokens.AsNoTracking().Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<UserToken> GetAuthenticationType(string email)
        {
            return await Db.UserTokens.AsNoTracking().Include(s=>s.User).Where(x => x.User.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> Remove(UserToken entity)
        {
            Db.UserTokens.Remove(entity);
            return await SaveChanges<UserToken>();
        }

        public async Task<bool> Update(UserToken entity)
        {
            var currentAuthenticationType = Db.UserTokens.Where(s=>s.UserId == entity.UserId).FirstOrDefault();

            Db.Entry(currentAuthenticationType).CurrentValues.SetValues(entity);

            Db.Entry(currentAuthenticationType).State = EntityState.Modified;

            return await SaveChanges<UserToken>();
        }
    }
}
