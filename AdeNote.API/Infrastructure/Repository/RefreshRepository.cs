using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class RefreshRepository : Repository, IRefreshTokenRepository
    {
        public RefreshRepository(NoteDbContext db):base(db) 
        { 

        }

        public async Task<bool> Add(RefreshToken entity)
        {
            entity.Id = Guid.NewGuid();
            await Db.RefreshTokens.AddAsync(entity);
            return await SaveChanges<RefreshToken>();
        }

        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var refreshTokens = await Db.RefreshTokens.ToListAsync();

            var currentRefreshToken = refreshTokens.FirstOrDefault(s => s.Token == refreshToken);

            return currentRefreshToken;
        }

        public async Task<RefreshToken> GetRefreshTokenByUserId(Guid userId, string refreshToken)
        {
            var refreshTokens = await Db.RefreshTokens.ToListAsync();

            var currentRefreshToken = refreshTokens.FirstOrDefault(s => s.UserId == userId && s.Token == refreshToken);

            return currentRefreshToken;
        }

        public async Task<Guid> GetUserByRefreshToken(string refreshToken)
        {
            return await Db.RefreshTokens.Where(s => s.Token == refreshToken).
                Select(s => s.UserId).FirstOrDefaultAsync();
        }

        public async Task<bool> Remove(RefreshToken entity)
        {
            Db.RefreshTokens.Remove(entity);
            return await SaveChanges<RefreshToken>();
        }

        public async Task<bool> Update(RefreshToken entity)
        {
            var currentUserDetail = Db.RefreshTokens.Where(s => s.Id == entity.Id).FirstOrDefault();

            Db.Entry(currentUserDetail).CurrentValues.SetValues(entity);

            Db.Entry(currentUserDetail).State = EntityState.Modified;

            return await SaveChanges<RefreshToken>();
        }
    }
}
