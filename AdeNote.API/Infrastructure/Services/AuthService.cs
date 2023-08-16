using AdeNote.Infrastructure.Repository;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Google.Authenticator;
using System.Text;
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
                byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");

                var authenticator = new TwoFactorAuthenticator()
                    .GenerateSetupCode("AdeNote", email, accountKey);

                var qrCode = authenticator.QrCodeSetupImageUrl.Split(',')[1];

                var imageBytes = Convert.FromBase64String(qrCode);

                var memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length)
                {
                    Position = 0
                };

                var url = await blobService.UploadFile("qrCode", memoryStream);

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

        public ActionResult<string> GenerateMFAToken(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return ActionResult<string>.Failed("Invalid email");

                var encodedToken = Encoding.UTF8.GetBytes($"{loginSecret}-{email}");
                var token  = Convert.ToBase64String(encodedToken);
                return ActionResult<string>.SuccessfulOperation(token);
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public ActionResult<string> ReadEmailFromToken(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    return ActionResult<string>.Failed("Invalid email");

                var mfaToken = Convert.FromBase64String(token);
                var decodedToken = Encoding.UTF8.GetString(mfaToken);
                var embededEmail = decodedToken.Split("-");
                if (!embededEmail.Contains(loginSecret))
                {
                    return ActionResult<string>.Failed("Invalid token");
                }
                return ActionResult<string>.SuccessfulOperation(embededEmail[1]);
            }
            catch (Exception ex)
            {

                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public async Task<ActionResult> IsAuthenticatorEnabled(string email)
        {
            try
            {
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

        private readonly string key;
        private IAuthRepository authRepository;
        private readonly IBlobService blobService;
        private readonly string loginSecret;
    }
}
