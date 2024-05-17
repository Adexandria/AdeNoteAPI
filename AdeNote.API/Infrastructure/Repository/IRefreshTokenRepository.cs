using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IRefreshTokenRepository :IRepository<RefreshToken>
    {
        Task<Guid> GetUserByRefreshToken(string refreshToken);

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
    }
}
