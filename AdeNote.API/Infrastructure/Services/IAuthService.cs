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
        /// Sets up MFA using google authenticator app
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">Email of the user</param>
        /// <returns>Manual key and qr code</returns>
        Task<ActionResult<AuthenticatorDTO>> SetAuthenticator(Guid userId,string email);

        /// <summary>
        /// Sets up MFA using sms authentication 
        /// </summary>
        /// <param name="userId">User id</param>
        Task<ActionResult> SetSmsAuthenticator(Guid userId);

        /// <summary>
        /// Sets up phone number
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="phoneNumber">phone number</param>
        Task<ActionResult> SetPhoneNumber(Guid userId,string phoneNumber);


        /// <summary>
        /// Sets up phone number
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="token">phone number verification token</param>
        Task<ActionResult> VerifyPhoneNumber(Guid userId, string token);


        /// <summary>
        /// Checks if a phone number has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>bool</returns>
        Task<ActionResult<bool>> IsPhoneNumberVerified(Guid userId);

        /// <summary>
        /// Sends otp to phone number 
        /// </summary>
        /// <param name="userId">User id</param>
        Task<ActionResult> SendSmsOTP(Guid userId,string email);

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
        /// Revokes existing refresh token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="refreshToken">Refresh token</param>
        Task<ActionResult> RevokeRefreshToken(Guid userId, string refreshToken);

        /// <summary>
        /// Checks if refresh token is revoked
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        Task<ActionResult> IsTokenRevoked(string refreshToken);

        /// <summary>
        /// Disables authentication for a user
        /// </summary>
        /// <param name="userId">User id</param>
        Task<ActionResult> DisableUserMFA(Guid userId);
    }
}
