using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;

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
        Task<ActionResult<string>> GetUserQrCode(Guid userId, CancellationToken cancellationToken = default);


        /// <summary>
        /// Checks if MFA has been enabled for the user using user id.
        /// </summary>
        /// <param name="email">Email of the user</param>
        Task<ActionResult<string>> IsAuthenticatorEnabled(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if MFA has been enabled for the user using user id.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="authenticatorType">Sms or google authenticator</param>
        Task<ActionResult> IsAuthenticatorEnabled(Guid userId, MFAType authenticatorType, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if MFA has been enabled for the user
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="authenticatorType">Sms or google authenticator</param>
        Task<ActionResult> IsAuthenticatorEnabled(string email, MFAType authenticatorType, CancellationToken cancellationToken = default);


        Task<ActionResult<string>> GenerateMFAToken(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets up MFA using google authenticator app
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">Email of the user</param>
        /// <returns>Manual key and qr code</returns>
        Task<ActionResult<AuthenticatorDTO>> SetAuthenticator(Guid userId,string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets up MFA using sms authentication 
        /// </summary>
        /// <param name="userId">User id</param>
        Task<ActionResult> SetSmsAuthenticator(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sets up phone number
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="phoneNumber">phone number</param>
        Task<ActionResult> SetPhoneNumber(Guid userId,string phoneNumber, CancellationToken cancellationToken = default);


        Task<ActionResult> ConfirmEmail(string verificationToken, CancellationToken cancellationToken = default);


        /// <summary>
        /// Sets up phone number
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="token">phone number verification token</param>
        Task<ActionResult> VerifyPhoneNumber(Guid userId, string token, CancellationToken cancellationToken = default);


        /// <summary>
        /// Checks if a phone number has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>bool</returns>
        Task<ActionResult> IsPhoneNumberVerified(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a email has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>bool</returns>
        Task<ActionResult<bool>> IsEmailVerified(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends otp to phone number 
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">Email</param>
        Task<ActionResult> SendSmsOTP(Guid userId,string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifies authenticator otp
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="otp">One time password</param>
        ActionResult VerifyAuthenticatorOTP(string email,string otp, CancellationToken cancellationToken = default);

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
        ActionResult<DetailsDTO> ReadDetailsFromToken(string token, CancellationToken cancellationToken = default);

        /// <summary>
        /// Revokes existing refresh token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="refreshToken">Refresh token</param>
        Task<ActionResult> RevokeRefreshToken(Guid userId, string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if refresh token is revoked
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        Task<ActionResult> IsTokenRevoked(string refreshToken, CancellationToken cancellationToken = default);

        /// <summary>
        /// Disables authentication for a user
        /// </summary>
        /// <param name="userId">User id</param>
        Task<ActionResult> DisableUserMFA(Guid userId, CancellationToken cancellationToken = default);

        Task<ActionResult> DisableUserMFA(string token, CancellationToken cancellationToken = default);

        Task<ActionResult<string>> GenerateResetPasswordToken(Guid userId, string email, CancellationToken cancellationToken = default);

        ActionResult VerifyResetToken(string token, CancellationToken cancellationToken = default);

        Task<ActionResult<string>> SignUser(CreateUserDTO newUser, AuthType authType = AuthType.local, Role role = Role.User);

        Task<ActionTokenResult<UserDTO>> LoginUser(LoginDTO login, AuthType authType, CancellationToken cancellationToken = default);

        Task<ActionResult<string>> GenerateAccessToken(string refreshToken, CancellationToken cancellationToken = default);

        Task<ActionResult<string>> GenerateAccessToken(Guid userId, string email, CancellationToken cancellationToken = default);

        Task<ActionResult<string>> LoginUserPasswordless(string email, CancellationToken cancellationToken = default);
        Task<ActionTokenResult<UserDTO>> VerifyPasswordlessToken(string token, CancellationToken cancellationToken = default);

        Task<ActionResult<string[]>> GenerateRecoveryCodes(Guid userId, CancellationToken cancellationToken = default);

        Task<ActionTokenResult<UserDTO>> LoginUserByRecoveryCodes(string[] recoveryCodes, CancellationToken cancellationToken = default);
    }
}
