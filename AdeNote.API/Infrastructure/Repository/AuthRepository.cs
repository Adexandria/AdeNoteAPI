using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;
using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Handles the authentication of the app
    /// </summary>
    public class AuthRepository : Repository, IAuthRepository
    {

        /// <summary>
        /// A constructor
        /// </summary>
        public AuthRepository()
        {
                
        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="noteDb">Handles the transactions</param>
        public AuthRepository(NoteDbContext db) : base(db)
        {
            
        }
        
        /// <summary>
        /// Saves a new entity
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Add(UserToken entity)
        {
            entity.Id = Guid.NewGuid();
            await Db.UserTokens.AddAsync(entity);
            return await SaveChanges<UserToken>();
        }

        /// <summary>
        /// Gets the authentication type of a user using user Id
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User token</returns>
        public async Task<UserToken> GetAuthenticationType(Guid userId)
        {
            return await Db.UserTokens.AsNoTracking().Where(x => x.UserId == userId)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the authentication type of a user using email
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <returns>user token</returns>
        public async Task<UserToken> GetAuthenticationType(string email)
        {
            return await Db.UserTokens.AsNoTracking().Include(s=>s.User).Where(x => x.User.Email == email)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Gets the refresh token by using the id
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Refresh token object</returns>
        public async Task<RefreshToken> GetRefreshToken(string refreshToken)
        {
            var refreshTokens = await Db.RefreshTokens.ToListAsync();

            var currentRefreshToken = refreshTokens.FirstOrDefault(s=> s.Token == refreshToken);

            return currentRefreshToken;
        }
        
        /// <summary>
        /// Gets the refresh token of a user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="refreshToken">token used to get the access token</param>
        /// <returns>Refresh token object</returns>
        public async Task<RefreshToken> GetRefreshTokenByUserId(Guid userId, string refreshToken)
        {
            var refreshTokens = await Db.RefreshTokens.ToListAsync();

            var currentRefreshToken = refreshTokens.FirstOrDefault(s => s.UserId.Id == userId && s.Token == refreshToken);

            return currentRefreshToken;
        }

        /// <summary>
        /// Remove an existing object
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Remove(UserToken entity)
        {
            Db.UserTokens.Remove(entity);
            return await SaveChanges<UserToken>();
        }

        /// <summary>
        /// Revokes the existing refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> RevokeRefreshToken(RefreshToken refreshToken)
        {
            refreshToken.IsRevoked = true;
            return await SaveChanges<RefreshToken>();
        }

        /// <summary>
        /// Updates an existing object
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Update(UserToken entity)
        {
            var currentAuthenticationType = Db.UserTokens.Where(s=>s.UserId == entity.UserId).FirstOrDefault();

            Db.Entry(currentAuthenticationType).CurrentValues.SetValues(entity);

            Db.Entry(currentAuthenticationType).State = EntityState.Modified;

            return await SaveChanges<UserToken>();
        }
    }
}
