using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Google.Authenticator;
using System.Text;
using TasksLibrary.Models;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        
        public AuthService(IAuthRepository _authRepository,IBlobService _blobService,IConfiguration _configuration)
        {
            authRepository = _authRepository;
            key = _configuration["TwoFactorSecret"];
            blobService = _blobService;
            loginSecret = _configuration["LoginSecret"];
        }
        public async Task<ActionResult<AuthenticatorDTO>> SetEmailAuthenticator(Guid userId, string email)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<AuthenticatorDTO>.Failed("Invalid user id");

                if(string.IsNullOrEmpty(email))
                    return ActionResult<AuthenticatorDTO>.Failed("Invalid email");

                byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");

                var authenticator = new TwoFactorAuthenticator()
                    .GenerateSetupCode("AdeNote", email, accountKey);

                var qrCode = authenticator.QrCodeSetupImageUrl.Split(',')[1];

                var imageBytes = Convert.FromBase64String(qrCode);

                var memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length)
                {
                    Position = 0
                };

                var imageName = new Guid().ToString("N")[^4];

                var url = await blobService.UploadFile($"qrCode{imageName}", memoryStream);

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

        public ActionResult VerifyEmailAuthenticator(string email, string otp)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return ActionResult<string>.Failed("Invalid email");

                if (string.IsNullOrEmpty(otp))
                    return ActionResult<string>.Failed("Invalid otp");

                byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");
                var authenticator = new TwoFactorAuthenticator();
                var result = authenticator
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

        public async Task<ActionResult<string>> GetUserQrCode(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id");

                var userAuthenticator = await authRepository.GetAuthenticationType(userId);

                if (userAuthenticator == null)
                    return ActionResult<string>.Failed("No two factor enabled");

                return ActionResult<string>.SuccessfulOperation(userAuthenticator.AuthenticatorKey);
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public async Task<ActionResult> IsAuthenticatorEnabled(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id");

                var userAuthenticator = await authRepository.GetAuthenticationType(userId);

                if(userAuthenticator == null)
                    return ActionResult<string>.Failed("No two factor enabled");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public ActionResult<string> GenerateMFAToken(Guid userId,string email,string refreshToken)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id");

                if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(refreshToken))
                    return ActionResult<string>.Failed("Invalid email or refresh token");

                var encodedToken = Encoding.UTF8.GetBytes($"{loginSecret}-{email}-{userId.ToString("N")}-{refreshToken}");
                var token  = Convert.ToBase64String(encodedToken);
                return ActionResult<string>.SuccessfulOperation(token);
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public ActionResult<DetailsDTO> ReadDetailsFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return ActionResult<DetailsDTO>.Failed("Invalid token");

                var mfaToken = Convert.FromBase64String(token);
                var decodedToken = Encoding.UTF8.GetString(mfaToken);
                var userDetails = decodedToken.Split("-");
                if (!userDetails.Contains(loginSecret))
                {
                    return ActionResult<DetailsDTO>.Failed("Invalid token");
                }

                var currentUserDetails = new DetailsDTO(userDetails[1], userDetails[2],userDetails[3]);
                return ActionResult<DetailsDTO>.SuccessfulOperation(currentUserDetails);
            }
            catch (Exception ex)
            {

                return ActionResult<DetailsDTO>.Failed(ex.Message);
            }
        }

        public async Task<ActionResult> IsAuthenticatorEnabled(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return ActionResult<AuthenticatorDTO>.Failed("Invalid email");

                var userAuthenticator = await authRepository.GetAuthenticationType(email);

                if (userAuthenticator == null)
                    return ActionResult<string>.Failed("No two factor enabled");

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public async Task<ActionResult> RevokeRefreshToken(Guid userId, string refreshToken)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id");

                if (string.IsNullOrEmpty(refreshToken))
                    return ActionResult<string>.Failed("Invalid refresh token");

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

        public async Task<ActionResult> IsTokenRevoked(string refreshToken)
        {
            try
            {

                if (string.IsNullOrEmpty(refreshToken))
                    return ActionResult.Failed("Invalid refresh token");

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

        private readonly string key;
        private IAuthRepository authRepository;
        private readonly IBlobService blobService;
        private readonly string loginSecret;
    }
}
