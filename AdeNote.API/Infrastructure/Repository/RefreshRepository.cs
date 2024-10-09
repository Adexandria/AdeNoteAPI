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
            var currentRefreshToken = await Db.RefreshTokens.FirstOrDefaultAsync(s => s.Token == refreshToken);

            return currentRefreshToken;
        }

        public async Task<bool> IsTokenRevoked(string refreshToken)
        {
           return Db.RefreshTokens.Any(s=>s.Token == refreshToken && s.IsRevoked);
        }

        public async Task<RefreshToken> GetRefreshTokenByUserId(Guid userId, string refreshToken)
        {
            var currentRefreshToken = await Db.RefreshTokens.FirstOrDefaultAsync(s => s.UserId == userId && s.Token == refreshToken);
            return currentRefreshToken;
        }

        public async Task<User> GetUserByRefreshToken(string refreshToken)
        {
            return await Db.RefreshTokens.Where(s => s.Token == refreshToken)
                .Include(s=>s.User).Select(s=>s.User).FirstOrDefaultAsync();
        }

        public async Task<bool> Remove(RefreshToken entity)
        {
            Db.RefreshTokens.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Remove refresh token from database: {result}", result);

            return result;
        }

        public async Task<bool> Update(RefreshToken entity)
        {
            Db.RefreshTokens.Update(entity);

            var result = await SaveChanges();

            logger.LogInformation("Update refresh token in database: {result}", result);

            return result;
        }
    }
}
