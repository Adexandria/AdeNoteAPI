using AdeNote.Models.DTOs;
using TasksLibrary.Models;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    /// <summary>
    /// An authentication interface that with the controller
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Gets user's Qr code
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Qr code url</returns>
        Task<ActionResult<string>> GetUserQrCode(Guid userId);

        /// <summary>
        /// Checks if MFA has been enabled for the user using user id.
        /// </summary>
        /// <param name="userId">User id</param>
        Task<ActionResult> IsAuthenticatorEnabled(Guid userId);
        /// <summary>
        /// Checks if MFA has been enabled for the user
        /// </summary>
        /// <param name="email">Email of the user</param>
        Task<ActionResult> IsAuthenticatorEnabled(string email);
        /// <summary>
        /// Sets up MFA using authenticator app
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">Email of the user</param>
        /// <returns>Manual key and qr code</returns>
        Task<ActionResult<AuthenticatorDTO>> SetAuthenticator(Guid userId,string email);      
        
        /// <summary>
        /// Verifies authenticator otp
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="otp">One time password</param>
        ActionResult VerifyAuthenticatorOTP(string email,string otp);

        /// <summary>
        /// Generates token when trying to log in using MFA
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">User's email</param>
        /// <param name="refreshToken">refresh token generated</param>
        /// <returns>Token</returns>
        ActionResult<string> GenerateMFAToken(Guid userId,string email,string refreshToken);

        /// <summary>
        /// Reads the user details from the token
        /// </summary>
        /// <param name="token">MFA Token</param>
        /// <returns>User details</returns>
        ActionResult<DetailsDTO> ReadDetailsFromToken(string token);

        /// <summary>
        /// Revoke existing refresh token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="refreshToken">Refresh token</param>
        Task<ActionResult> RevokeRefreshToken(Guid userId, string refreshToken);

        /// <summary>
        /// Check if refresh token is revoked
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        Task<ActionResult> IsTokenRevoked(string refreshToken);
    }
}
