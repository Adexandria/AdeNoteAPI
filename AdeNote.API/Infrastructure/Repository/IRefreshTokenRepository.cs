using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IRefreshTokenRepository :IRepository<RefreshToken>
    {
        Task<User> GetUserByRefreshToken(string refreshToken);

        /// <summary>
        /// Gets the refresh token of a user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="refreshToken">token used to get the access token</param>
        /// <returns>Refresh token object</returns>
        Task<RefreshToken> GetRefreshTokenByUserId(Guid userId, string refreshToken);

        /// <summary>
        /// Gets the refresh token by using the id
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Refresh token object</returns>
        Task<RefreshToken> GetRefreshToken(string refreshToken);

        /// <summary>
        /// Checks if token has been revoked
        /// </summary>
        /// <param name="refreshToken">refresh token</param>
        /// <returns>return true if it has revoked and false if otherwise</returns>
        Task<bool> IsTokenRevoked(string refreshToken);
    }
}
