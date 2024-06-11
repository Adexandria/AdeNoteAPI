using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class RefreshRepository : Repository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshRepository(NoteDbContext db, ILoggerFactory loggerFactory):base(db,loggerFactory) 
        { 
            
        }

        public async Task<bool> Add(RefreshToken entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.RefreshTokens.AddAsync(entity);

            var result =  await SaveChanges();

            logger.LogInformation("Add refresh token to database: {result}", result);

            return result;
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

            var result = await SaveChanges();

            logger.LogInformation("Remove refresh token to database: {result}", result);

            return result;
        }

        public async Task<bool> Update(RefreshToken entity)
        {
            Db.RefreshTokens.Update(entity);

            var result = await SaveChanges();

            logger.LogInformation("Update refresh token to database: {result}", result);

            return result;
        }
    }
}
