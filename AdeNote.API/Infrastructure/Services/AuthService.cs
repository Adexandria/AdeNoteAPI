using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Google.Authenticator;
using System.Text;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services 
{ 

    /// <summary>
    /// Implementation of the interface
    /// </summary>
    public class AuthService : IAuthService
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public AuthService()
        {
            TwoFactorAuthenticator = new TwoFactorAuthenticator();
        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="_authRepository">Handles the authentication</param>
        /// <param name="_blobService">Handles the cloud storage</param>
        /// <param name="_configuration">Reads the key/value pair from appsettings</param>
        public AuthService(IAuthRepository _authRepository,IBlobService _blobService,IConfiguration _configuration)
        {
            authRepository = _authRepository;
            key = _configuration["TwoFactorSecret"];
            blobService = _blobService;
            loginSecret = _configuration["LoginSecret"];
            TwoFactorAuthenticator = new TwoFactorAuthenticator();
        }
        /// <summary>
        /// Sets up MFA using authenticator app
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">Email of the user</param>
        /// <returns>Manual key and qr code</returns>
        public async Task<ActionResult<AuthenticatorDTO>> SetAuthenticator(Guid userId, string email)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<AuthenticatorDTO>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                if(string.IsNullOrEmpty(email))
                    return ActionResult<AuthenticatorDTO>.Failed("Invalid email", StatusCodes.Status404NotFound);

                byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");

                var authenticator = TwoFactorAuthenticator
                    .GenerateSetupCode("AdeNote", email, accountKey);

                var qrCode = authenticator.QrCodeSetupImageUrl.Split(',')[1];

                var imageBytes = Convert.FromBase64String(qrCode);

                var memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length)
                {
                    Position = 0
                };

                var imageName = new Guid().ToString("N")[^4];

                var url = await blobService.UploadImage($"qrCode{imageName}", memoryStream);

                var userToken = new UserToken(MFAType.AuthenicationApp, userId).SetAuthenticatorKey(url);

                var result = await authRepository.Add(userToken);
                if (!result)
                    return ActionResult<AuthenticatorDTO>.Failed("failed to set up two factor authentication",StatusCodes.Status400BadRequest);

                var authenticatorDto = new AuthenticatorDTO(authenticator.ManualEntryKey,url);

                return ActionResult<AuthenticatorDTO>.SuccessfulOperation(authenticatorDto);
            }
            catch (Exception ex)
            {
                return ActionResult<AuthenticatorDTO>.Failed(ex.Message);
            }
            
        }
        
        /// <summary>
        /// Verifies authenticator otp
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="otp">One time password</param>
        public ActionResult VerifyAuthenticatorOTP(string email, string otp)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return ActionResult<string>.Failed("Invalid email", StatusCodes.Status404NotFound);

                if (string.IsNullOrEmpty(otp))
                    return ActionResult<string>.Failed("Invalid otp", StatusCodes.Status404NotFound);

                byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");
                var result = TwoFactorAuthenticator
                    .ValidateTwoFactorPIN(accountKey, otp);
                if (!result)
                    return ActionResult.Failed("Invalid otp", StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }
        
        /// <summary>
        /// Gets user's Qr code
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Qr code url</returns>
        public async Task<ActionResult<string>> GetUserQrCode(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                var userAuthenticator = await authRepository.GetAuthenticationType(userId);

                if (userAuthenticator == null)
                    return ActionResult<string>.Failed("No two factor enabled",StatusCodes.Status400BadRequest);

                return ActionResult<string>.SuccessfulOperation(userAuthenticator.AuthenticatorKey);
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        /// <summary>
        /// Checks if MFA has been enabled for the user using user id.
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> IsAuthenticatorEnabled(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id",StatusCodes.Status404NotFound);

                var userAuthenticator = await authRepository.GetAuthenticationType(userId);

                if(userAuthenticator == null)
                    return ActionResult<string>.Failed("No two factor enabled",StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }
        
        /// <summary>
        /// Generates token when trying to log in using MFA
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">User's email</param>
        /// <param name="refreshToken">refresh token generated</param>
        /// <returns>Token</returns>
        public ActionResult<string> GenerateMFAToken(Guid userId,string email,string refreshToken)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(refreshToken))
                    return ActionResult<string>.Failed("Invalid email or refresh token",StatusCodes.Status400BadRequest);

                var encodedToken = Encoding.UTF8.GetBytes($"{loginSecret}-{email}-{userId:N}-{refreshToken}");
                var token  = Convert.ToBase64String(encodedToken);
                return ActionResult<string>.SuccessfulOperation(token);
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }
        
        /// <summary>
        /// Reads the user details from the token
        /// </summary>
        /// <param name="token">MFA Token</param>
        /// <returns>User details</returns>
        public ActionResult<DetailsDTO> ReadDetailsFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return ActionResult<DetailsDTO>.Failed("Invalid token",StatusCodes.Status404NotFound);

                var mfaToken = Convert.FromBase64String(token);
                
                var decodedToken = Encoding.UTF8.GetString(mfaToken);
                
                var userDetails = decodedToken.Split("-");
                
                if (!userDetails.Contains(loginSecret))
                {
                    return ActionResult<DetailsDTO>.Failed("Invalid token",StatusCodes.Status400BadRequest);
                }

                var currentUserDetails = new DetailsDTO(userDetails[1], userDetails[2],userDetails[3]);
                
                return ActionResult<DetailsDTO>.SuccessfulOperation(currentUserDetails);
            }
            catch (Exception ex)
            {

                return ActionResult<DetailsDTO>.Failed(ex.Message);
            }
        }

        /// <summary>
        /// Checks if MFA has been enabled for the user
        /// </summary>
        /// <param name="email">Email of the user</param>
        public async Task<ActionResult> IsAuthenticatorEnabled(string email)
        {
            try
            {
               if (string.IsNullOrEmpty(email))
                    return ActionResult<AuthenticatorDTO>.Failed("Invalid email", StatusCodes.Status404NotFound);

                var userAuthenticator = await authRepository.GetAuthenticationType(email);

                if (userAuthenticator == null)
                    return ActionResult<string>.Failed("No two factor enabled", StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }
        
        /// <summary>
        /// Revoke existing refresh token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="refreshToken">Refresh token</param>
        public async Task<ActionResult> RevokeRefreshToken(Guid userId, string refreshToken)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                if (string.IsNullOrEmpty(refreshToken))
                    return ActionResult<string>.Failed("Invalid refresh token",StatusCodes.Status404NotFound);

                var currentRefreshToken = await authRepository.GetRefreshTokenByUserId(userId, refreshToken);

                if (currentRefreshToken == null)
                    return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);

                await authRepository.RevokeRefreshToken(currentRefreshToken);
                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// Check if refresh token is revoked
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        public async Task<ActionResult> IsTokenRevoked(string refreshToken)
        {
            try
            {

                if (string.IsNullOrEmpty(refreshToken))
                    return ActionResult.Failed("Invalid refresh token", StatusCodes.Status404NotFound);

                var currentRefreshToken = await authRepository.GetRefreshToken(refreshToken);

                if (currentRefreshToken == null || currentRefreshToken.IsRevoked)
                    return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public string key;
        public IAuthRepository authRepository;
        public IBlobService blobService;
        public string loginSecret;
        public TwoFactorAuthenticator TwoFactorAuthenticator;
    }
}
