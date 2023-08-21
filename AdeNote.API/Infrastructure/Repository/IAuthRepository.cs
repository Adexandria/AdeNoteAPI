using AdeNote.Models;
using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// An interface that handles the authentication of an application
    /// </summary>
    public interface IAuthRepository : IRepository<UserToken>
    {
        /// <summary>
        /// Gets the authentication type of a user using user Id
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User token</returns>
        Task<UserToken> GetAuthenticationType(Guid userId);

        /// <summary>
        /// Gets the authentication type of a user using email
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <returns>user token</returns>
        Task<UserToken> GetAuthenticationType(string email);

        /// <summary>
        /// Gets the refresh token of a user
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="refreshToken">token used to get the access token</param>
        /// <returns>Refresh token object</returns>
        Task<RefreshToken> GetRefreshTokenByUserId(Guid userId,string refreshToken);

        /// <summary>
        /// Gets the refresh token by using the id
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        /// <returns>Refresh token object</returns>
        Task<RefreshToken> GetRefreshToken(string refreshToken);


        /// <summary>
        /// Revokes the existing refresh token
        /// </summary>
        /// <param name="refreshToken">Refresh token object</param>
        /// <returns>a boolean value</returns>
        Task<bool> RevokeRefreshToken(RefreshToken refreshToken);
    }
}
