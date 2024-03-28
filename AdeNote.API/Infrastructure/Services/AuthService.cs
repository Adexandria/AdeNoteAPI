using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Autofac;
using Google.Authenticator;
using System.Text;
using TasksLibrary.Services;
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
        /// <param name="_smsService"> A sms service used to send message via sms</param>
        /// <param name="_userDetailRepository">Handles the details of a user</param>
        public AuthService(IAuthRepository _authRepository,
            IUserDetailRepository _userDetailRepository,
            IBlobService _blobService,IConfiguration _configuration,
            ISmsService _smsService,
            INotificationService notificationService,AuthTokenRepository authTokenRepository)
        {
            authRepository = _authRepository;
            userDetailRepository = _userDetailRepository;
            smsService = _smsService;
            key = _configuration["TwoFactorSecret"];
            blobService = _blobService;
            loginSecret = _configuration["LoginSecret"];
            TwoFactorAuthenticator = new TwoFactorAuthenticator();
            tokenRepository = authTokenRepository;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Sets up MFA using google authenticator app
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

                var imageName = Guid.NewGuid().ToString("N")[^4];

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
        /// Sets phone number verification
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="phoneNumber">phone number</param>
        public async Task<ActionResult> SetPhoneNumber(Guid userId, string phoneNumber)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

                if (string.IsNullOrEmpty(phoneNumber))
                    return ActionResult.Failed("Invalid phone number", StatusCodes.Status404NotFound);

                var userDetails = new UserDetail(userId, phoneNumber);

                var result = await userDetailRepository.Add(userDetails);
                if (!result)
                    return ActionResult.Failed("Failed to set up sms authenticator", StatusCodes.Status400BadRequest);

                var bytesKey = Encoding.UTF8.GetBytes($"{key}-{phoneNumber}");

                var token = BitConverter.ToInt16(bytesKey, 0).ToString();
                var message = $"Enter your authentication code {token} to verify your phone number";

                smsService.SendSms(new Sms(phoneNumber,message));

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
          
        }

        /// <summary>
        /// Checks if a phone number has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>boolean value</returns>
        public async Task<ActionResult<bool>> IsPhoneNumberVerified(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<bool>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                var isVerified = await userDetailRepository.IsPhoneNumberVerified(userId);

                if (!isVerified.HasValue)
                    return ActionResult<bool>.Failed("user details doesn't exist", StatusCodes.Status400BadRequest);

                return ActionResult<bool>.SuccessfulOperation(isVerified.Value);
            }
            catch (Exception ex)
            {
                return ActionResult < bool>.Failed(ex.Message);
            }
            
        }

        /// <summary>
        /// Sets up phone number
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="token">phone number verification token</param>
        public async Task<ActionResult> VerifyPhoneNumber(Guid userId, string token)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

                if (string.IsNullOrEmpty(token))
                    return ActionResult.Failed("Invalid phone number", StatusCodes.Status404NotFound);

                var currentUserDetail = await userDetailRepository.GetUserDetail(userId);
                if(currentUserDetail == null)
                    return ActionResult.Failed("User details not found", StatusCodes.Status400BadRequest);


                var bytesKey = Encoding.UTF8.GetBytes($"{key}-{currentUserDetail.Phonenumber}");

                var generatedToken = BitConverter.ToInt16(bytesKey, 0).ToString();

                if(generatedToken != token)
                    return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);

                currentUserDetail.VerifyPhoneNumber();

                var result = await userDetailRepository.Update(currentUserDetail);
                if(!result)
                    return ActionResult.Failed("Failed to verify phonenumber", StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }


        /// <summary>
        /// Sets up MFA using sms authentication 
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> SetSmsAuthenticator(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

                var userToken = new UserToken(MFAType.Sms, userId);

                var result = await authRepository.Add(userToken);
                if (!result)
                    return ActionResult.Failed("failed to set up two factor authentication", StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// Sends otp to phone number 
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> SendSmsOTP(Guid userId,string email)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

                var currentUserDetail = await userDetailRepository.GetUserDetail(userId);
                if (currentUserDetail == null)
                    return ActionResult.Failed("User details not found", StatusCodes.Status400BadRequest);

                byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");

                var authenticator = TwoFactorAuthenticator
                 .GenerateSetupCode("AdeNote", currentUserDetail.User.Email, accountKey);

                var generatedToken = TwoFactorAuthenticator.GetCurrentPIN(accountKey, DateTime.UtcNow.AddMinutes(3));

                var message = $"Hello, your AdeNote secure code is {generatedToken}. DO NOT SHARE this with anyone. Only valid for 3 minutes";

                smsService.SendSms(new Sms(currentUserDetail.Phonenumber, message));

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
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
        /// Revokes existing refresh token
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
        /// Checks if refresh token is revoked
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

        /// <summary>
        /// Disables Multi factor authentication for a particular user
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> DisableUserMFA(Guid userId)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                var authenticationType = await authRepository.GetAuthenticationType(userId);
                if(authenticationType == null)
                    return ActionResult.Failed("MFA isn't enabled for user", StatusCodes.Status400BadRequest);

                if(authenticationType.AuthenticationType == MFAType.AuthenicationApp)
                {
                    var isDeleted = await blobService.DeleteImage(authenticationType.AuthenticatorKey);
                    if (!isDeleted)
                        return ActionResult.Failed("Failed to delete qr code");
                }

               var result =  await authRepository.Remove(authenticationType);
                if(!result)
                    return ActionResult.Failed("Failed to remove user token", StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        public async Task<ActionResult<string>> GenerateResetToken(Guid userId, string email)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                var token = tokenRepository.GenerateAccessToken(userId, email);

                var substitutions = new Dictionary<string, string>()
                {
                    {"[TOKEN]" , token }
                };

                await _notificationService.SendNotification(new Email(email, "Password Reset Token"),
                    EmailTemplate.ResetTokenNotification, ContentType.html, substitutions);

                return ActionResult<string>.SuccessfulOperation(token);
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public ActionResult VerifyResetToken(string token)
        {
            try
            {
                if(string.IsNullOrEmpty(token))
                    return ActionResult.Failed("Invalid token", StatusCodes.Status404NotFound);

                var userDTO = tokenRepository.VerifyToken(token);

                if (userDTO == null)
                    return ActionResult.Failed("Failed to verify token", StatusCodes.Status400BadRequest);

                return ActionResult.Successful();
            }
            catch (Exception ex)
            {
                return ActionResult.Failed(ex.Message);
            }
        }

        /// <summary>
        /// Two factor authentication secret key
        /// </summary>
        public string key;

        /// <summary>
        /// Handles the authentication
        /// </summary>
        public IAuthRepository authRepository;

        /// <summary>
        /// Handles the details of a user
        /// </summary>
        public IUserDetailRepository userDetailRepository;

        /// <summary>
        /// A sms service used to send message via sms
        /// </summary>
        public ISmsService smsService;

        /// <summary>
        ///  Handles the cloud storage
        /// </summary>
        public IBlobService blobService;

        /// <summary>
        /// Secret to generate multifactor token
        /// </summary>
        public string loginSecret;

        /// <summary>
        /// Object used to generate and set up two factor authentication
        /// </summary>
        public TwoFactorAuthenticator TwoFactorAuthenticator;

        public AuthTokenRepository tokenRepository;

        public INotificationService _notificationService;
    }
}
