using AdeAuth.Services.Interfaces;
using AdeNote.Infrastructure.Repository;
using AdeNote.Infrastructure.Services.Blob;
using AdeNote.Infrastructure.Services.Notification;
using AdeNote.Infrastructure.Services.SmsSettings;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EmailSettings;
using AdeNote.Infrastructure.Utilities.SmsConfig;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using System.Security.Claims;
using System.Text;

namespace AdeNote.Infrastructure.Services.Authentication
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

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="_blobService">Handles the cloud storage</param>
        /// <param name="_configuration">Reads the key/value pair from appsettings</param>
        /// <param name="_smsService"> A sms service used to send message via sms</param>
        /// <param name="notificationService"></param>
        /// <param name="_userRepository"></param>
        /// <param name="_passwordManager"></param>
        /// <param name="_refreshTokenRepository"></param>
        /// <param name="_tokenProvider"></param>
        public AuthService(IUserRepository _userRepository,
            ITokenProvider _tokenProvider, IMfaService _mfaService,
            IBlobService _blobService, IConfiguration _configuration,
            IRefreshTokenRepository _refreshTokenRepository,
            IPasswordManager _passwordManager,
            ISmsService _smsService,
            IUserService<User> _userService,
            INotificationService notificationService, IRecoveryCodeRepository _recoveryCodeRepository)
        {
            smsService = _smsService;
            key = _configuration["TwoFactorSecret"];
            userRepository = _userRepository;
            tokenProvider = _tokenProvider;
            mfaService = _mfaService;
            blobService = _blobService;
            refreshTokenRepository = _refreshTokenRepository;
            passwordManager = _passwordManager;
            loginSecret = _configuration["LoginSecret"];
            _tokenProvider.SetTokenEncryptionKey(_configuration["TokenSecret"]);
            _notificationService = notificationService;
            recoveryCodeRepository = _recoveryCodeRepository;

            userService = _userService;
        }

        /// <summary>
        /// Sets up MFA using google authenticator app
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">Email of the user</param>
        /// <returns>Manual key and qr code</returns>
        public async Task<ActionResult<AuthenticatorDTO>> SetAuthenticator(Guid userId, string email, CancellationToken cancellationToken)
        {

            if (userId == Guid.Empty)
                return ActionResult<AuthenticatorDTO>.Failed("Invalid user id", StatusCodes.Status404NotFound);

            if (string.IsNullOrEmpty(email))
                return ActionResult<AuthenticatorDTO>.Failed("Invalid email", StatusCodes.Status404NotFound);

            var currentUser = await userRepository.GetUser(userId);

            if (currentUser == null)
                return ActionResult<AuthenticatorDTO>.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");

            var authenticator = mfaService.SetupGoogleAuthenticator("Adenote", email, accountKey);

            var qrCode = authenticator.QrCodeImage.Split(',')[1];

            var imageBytes = Convert.FromBase64String(qrCode);

            var memoryStream = new MemoryStream(imageBytes, 0, imageBytes.Length)
            {
                Position = 0
            };

            var imageName = Guid.NewGuid().ToString("N")[^4];

            cancellationToken.ThrowIfCancellationRequested();

            var imagePath = await blobService.UploadImage($"qrCode{imageName}", memoryStream,cancellationToken);

            if( imagePath != "Success")
            {
                return ActionResult<AuthenticatorDTO>.Failed(imagePath, StatusCodes.Status400BadRequest);
            }

            currentUser.EnableTwoFactor(MFAType.google, imagePath);

            var result = await userRepository.Update(currentUser);
            if (!result)
                return ActionResult<AuthenticatorDTO>.Failed("failed to set up two factor authentication", StatusCodes.Status400BadRequest);

            var authenticatorDto = new AuthenticatorDTO(authenticator.ManualKey, imagePath);

            return ActionResult<AuthenticatorDTO>.SuccessfulOperation(authenticatorDto);
        }

        /// <summary>
        /// Sets phone number verification
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="phoneNumber">phone number</param>
        public async Task<ActionResult> SetPhoneNumber(Guid userId, string phoneNumber)
        {
            if (userId == Guid.Empty)
                return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

            if (string.IsNullOrEmpty(phoneNumber))
                return ActionResult.Failed("Invalid phone number", StatusCodes.Status404NotFound);

            var currentUser = await userRepository.GetUser(userId);

            if (currentUser == null)
                return ActionResult<AuthenticatorDTO>.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            currentUser.PhoneNumber = phoneNumber;

            var result = await userRepository.Update(currentUser);
            if (!result)
                return ActionResult.Failed("Failed to set up sms authenticator", StatusCodes.Status400BadRequest);

            var bytesKey = Encoding.UTF8.GetBytes($"{key}-{phoneNumber}");

            var token = tokenProvider.GenerateOTP(bytesKey);

            var message = $"Enter your authentication code {token} to verify your phone number";

            smsService.SendSms(new Sms(phoneNumber, message));

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Checks if a phone number has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>boolean value</returns>
        public async Task<ActionResult> IsPhoneNumberVerified(Guid userId)
        {
            if (userId == Guid.Empty)
                return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

            var isVerified = await userRepository.IsPhoneNumberVerified(userId);

            if (!isVerified)
                return ActionResult.Failed("Phone number has not been verified", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Sets up phone number
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="token">phone number verification token</param>
        public async Task<ActionResult> VerifyPhoneNumber(Guid userId, string token)
        {
            if (userId == Guid.Empty)
                return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

            var currentUser = await userRepository.GetUser(userId);
            if (currentUser == null)
                return ActionResult.Failed("User details not found", StatusCodes.Status400BadRequest);


            var bytesKey = Encoding.UTF8.GetBytes($"{key}-{currentUser.PhoneNumber}");

            var isVerified = tokenProvider.VerifyOTP(bytesKey, token);

            if (isVerified)
                return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);

            currentUser.ConfirmPhoneNumberVerification();

            var result = await userRepository.Update(currentUser);
            if (!result)
                return ActionResult.Failed("Failed to verify phonenumber", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();
        }


        /// <summary>
        /// Sets up MFA using sms authentication 
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> SetSmsAuthenticator(Guid userId)
        {
            if (userId == Guid.Empty)
                return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

            var currentUser = await userRepository.GetUser(userId);

            if (currentUser == null)
                return ActionResult<AuthenticatorDTO>.Failed("User doesn't exist", StatusCodes.Status404NotFound);

            currentUser.EnableTwoFactor(MFAType.sms);

            var result = await userRepository.Update(currentUser);
            if (!result)
                return ActionResult.Failed("failed to set up two factor authentication", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();
        }

        /// <summary>
        /// Sends otp to phone number 
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> SendSmsOTP(Guid userId, string email)
        {
            if (userId == Guid.Empty)
                return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

            var currentUser = await userRepository.GetUser(userId);
            if (currentUser == null)
                return ActionResult.Failed("User details not found", StatusCodes.Status400BadRequest);

            byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");

            mfaService.SetupGoogleAuthenticator("Adenote", email, accountKey);
            
            var generatedToken = mfaService.GenerateGoogleAuthenticatorPin(accountKey, DateTime.UtcNow.AddMinutes(3));

            var message = $"Hello, your AdeNote secure code is {generatedToken}. DO NOT SHARE this with anyone. Only valid for 3 minutes";

            smsService.SendSms(new Sms(currentUser.PhoneNumber, message));

            return ActionResult.SuccessfulOperation();
        }

        /// <summary>
        /// Verifies authenticator otp
        /// </summary>
        /// <param name="email">Email of the user</param>
        /// <param name="otp">One time password</param>
        public ActionResult VerifyAuthenticatorOTP(string email, string otp)
        {
            if (string.IsNullOrEmpty(email))
                return ActionResult.Failed("Invalid email", StatusCodes.Status404NotFound);

            if (string.IsNullOrEmpty(otp))
                return ActionResult.Failed("Invalid otp", StatusCodes.Status404NotFound);

            byte[] accountKey = Encoding.ASCII.GetBytes($"{key}-{email}");

            var result = mfaService.VerifyGoogleAuthenticatorTotp(otp, accountKey);

            if (!result)
                return ActionResult.Failed("Invalid otp", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();
        }

        /// <summary>
        /// Gets user's Qr code
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>Qr code url</returns>
        public async Task<ActionResult<string>> GetUserQrCode(Guid userId)
        {
            if (userId == Guid.Empty)
                return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

            var user = await userRepository.GetUser(userId);

            if (user == null)
                return ActionResult<string>.Failed("No two factor enabled", StatusCodes.Status400BadRequest);

            return ActionResult<string>.SuccessfulOperation(user.AuthenticatorKey);
        }

        /// <summary>
        /// Checks if MFA has been enabled for the user using user id.
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> IsAuthenticatorEnabled(Guid userId, MFAType authenticatorType)
        {
            if (userId == Guid.Empty)
                return ActionResult.Failed("Invalid user id", StatusCodes.Status404NotFound);

            var userAuthenticator = await userRepository.GetUser(userId);

            if (userAuthenticator == null)
                return ActionResult.Failed("No two factor enabled", StatusCodes.Status400BadRequest);

            if (userAuthenticator.TwoFactorType != (int)authenticatorType)
                return ActionResult<string>.Failed("Invalid authenticator type", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();
        }

        /// <summary>
        /// Generates token when trying to log in using MFA
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="email">User's email</param>
        /// <param name="refreshToken">refresh token generated</param>
        /// <returns>Token</returns>
        public ActionResult<string> GenerateMFAToken(Guid userId, string email, string refreshToken)
        {
            if (userId == Guid.Empty)
                return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(refreshToken))
                return ActionResult<string>.Failed("Invalid email or refresh token", StatusCodes.Status400BadRequest);

            var encodedToken = Encoding.UTF8.GetBytes($"{loginSecret}-{email}-{userId:N}-{refreshToken}");

            var token = tokenProvider.GenerateToken(encodedToken);

            return ActionResult<string>.SuccessfulOperation(token);
        }

        /// <summary>
        /// Reads the user details from the token
        /// </summary>
        /// <param name="token">MFA Token</param>
        /// <returns>User details</returns>
        public ActionResult<DetailsDTO> ReadDetailsFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return ActionResult<DetailsDTO>.Failed("Invalid token", StatusCodes.Status404NotFound);

            var decodedToken = tokenProvider.ReadToken(token, "-");

            if (!decodedToken.Contains(loginSecret))
            {
                return ActionResult<DetailsDTO>.Failed("Invalid token", StatusCodes.Status400BadRequest);
            }

            var userDetails = new DetailsDTO(decodedToken[1], decodedToken[2], decodedToken[3]);

            return ActionResult<DetailsDTO>.SuccessfulOperation(userDetails);
        }

        /// <summary>
        /// Checks if MFA has been enabled for the user
        /// </summary>
        /// <param name="email">Email of the user</param>
        public async Task<ActionResult> IsAuthenticatorEnabled(string email, MFAType authenticatorType)
        {

            if (string.IsNullOrEmpty(email))
                return ActionResult<AuthenticatorDTO>.Failed("Invalid email", StatusCodes.Status404NotFound);

            var userAuthenticator = await userRepository.GetUserByEmail(email);

            if (userAuthenticator == null)
                return ActionResult<string>.Failed("No two factor enabled", StatusCodes.Status400BadRequest);

            if (userAuthenticator.TwoFactorType != (int)authenticatorType)
                return ActionResult<string>.Failed("Invalid authenticator type", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();

        }

        public async Task<ActionResult<string>> GenerateMFAToken(string email)
        {
            if (string.IsNullOrEmpty(email))
                return ActionResult<string>.Failed("Invalid email", StatusCodes.Status404NotFound);

            var user = await userRepository.GetUserByEmail(email);

            if (user == null)
                return ActionResult<string>.Failed("No email associated with a user", StatusCodes.Status400BadRequest);

            var token = tokenProvider.GenerateToken(new Dictionary<string, object>() { { "id", user.Id.ToString("N") },
                { ClaimTypes.Email,user.Email} }, 10);

            var substitutions = new Dictionary<string, string>()
                {
                    {"[Token]" , token }
                };

            _notificationService.SendNotification(new Email(email, "Multi-Factor Removal Token"),
               EmailTemplate.MfaRemovalTokenNotification, ContentType.html, substitutions);

            return ActionResult<string>.SuccessfulOperation(token);
        }


        /// <summary>
        /// Revokes existing refresh token
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="refreshToken">Refresh token</param>
        public async Task<ActionResult> RevokeRefreshToken(Guid userId, string refreshToken)
        {
            if (userId == Guid.Empty)
                return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

            if (string.IsNullOrEmpty(refreshToken))
                return ActionResult<string>.Failed("Invalid refresh token", StatusCodes.Status404NotFound);

            var currentRefreshToken = await refreshTokenRepository.GetRefreshTokenByUserId(userId, refreshToken);

            if (currentRefreshToken == null)
                return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);

            currentRefreshToken.RevokeToken();

            var result = await refreshTokenRepository.Update(currentRefreshToken);

            if (!result)
            {
                return ActionResult.Failed("Failed too revoke refresh token");
            }

            return ActionResult.SuccessfulOperation();
        }

        /// <summary>
        /// Checks if refresh token is revoked
        /// </summary>
        /// <param name="refreshToken">Refresh token</param>
        public async Task<ActionResult> IsTokenRevoked(string refreshToken)
        {

            if (string.IsNullOrEmpty(refreshToken))
                return ActionResult.Failed("Invalid refresh token", StatusCodes.Status404NotFound);

            var currentRefreshToken = await refreshTokenRepository.GetRefreshToken(refreshToken);

            if (currentRefreshToken == null || currentRefreshToken.IsRevoked)
                return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();

        }

        /// <summary>
        /// Disables Multi factor authentication for a particular user
        /// </summary>
        /// <param name="userId">User id</param>
        public async Task<ActionResult> DisableUserMFA(Guid userId)
        {
            if (userId == Guid.Empty)
                return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

            var user = await userRepository.GetUser(userId);
            if (user == null)
                return ActionResult.Failed("MFA isn't enabled for user", StatusCodes.Status400BadRequest);

            if (user.TwoFactorType == (int)MFAType.google)
            {
                var isDeleted = await blobService.DeleteImage(user.AuthenticatorKey);
                if (!isDeleted)
                    return ActionResult.Failed("Failed to delete qr code");
            }
            user.DisableTwoFactor();

            var result = await userRepository.Update(user);
            if (!result)
                return ActionResult.Failed("Failed to remove user token", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();
        }


        public async Task<ActionResult> DisableUserMFA(string token)
        {
            if (string.IsNullOrEmpty(token))
                return ActionResult.Failed("Invalid token", StatusCodes.Status404NotFound);

            var claimValues = tokenProvider.ReadToken(token, true, "id", ClaimTypes.Email);

            if (claimValues == null)
                return ActionResult.Failed("Failed to verify token", StatusCodes.Status400BadRequest);

            var currentUser = await userRepository.GetUser((Guid)claimValues.GetValueOrDefault("id"));
            if (currentUser == null)
                return ActionResult.Failed("MFA isn't enabled for user", StatusCodes.Status400BadRequest);

            if (currentUser.TwoFactorType == (int)MFAType.google)
            {
                var isDeleted = await blobService.DeleteImage(currentUser.AuthenticatorKey);
                if (!isDeleted)
                    return ActionResult.Failed("Failed to delete qr code");
            }

            currentUser.DisableTwoFactor();

            var result = await userRepository.Update(currentUser);

            if (!result)
                return ActionResult.Failed("Failed to remove user token", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();

        }

        public async Task<ActionResult<string>> GenerateResetPasswordToken(Guid userId, string email)
        {
            try
            {
                if (userId == Guid.Empty)
                    return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status404NotFound);

                var token = tokenProvider.GenerateToken(new Dictionary<string, object>() { { "id", userId.ToString("N") },
                { ClaimTypes.Email,email} }, 30);

                var substitutions = new Dictionary<string, string>()
                {
                    {"[TOKEN]" , token }
                };

                _notificationService.SendNotification(new Email(email, "Password Reset Token"),
                    EmailTemplate.ResetTokenNotification, ContentType.html, substitutions);

                return ActionResult<string>.SuccessfulOperation(token);
            }
            catch (OperationCanceledException)
            {
                return ActionResult<string>
                    .Failed("Reset password generation has been cancelled", StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        public ActionResult VerifyResetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return ActionResult.Failed("Invalid token", StatusCodes.Status404NotFound);

            var userDTO = tokenProvider.ReadToken(token, true, "id", ClaimTypes.Email);

            if (userDTO == null)
                return ActionResult.Failed("Failed to verify token", StatusCodes.Status400BadRequest);

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult<string>> IsAuthenticatorEnabled(string email)
        {
            if (string.IsNullOrEmpty(email))
                return ActionResult<string>.Failed("Invalid email", StatusCodes.Status404NotFound);

            var currentUser = await userRepository.GetUserByEmail(email);

            if (currentUser == null)
                return ActionResult<string>.Failed("No two factor enabled", StatusCodes.Status400BadRequest);


            var authenticationType = (MFAType)currentUser.TwoFactorType;

            return ActionResult<string>.SuccessfulOperation(authenticationType.ToString());
        }

        public async Task<ActionResult<string>> SignUser(CreateUserDTO newUser, AuthType authType, Role role)
        {
            var isEmailExist = userRepository.IsExist(newUser.Email);
            if (isEmailExist)
            {
                return ActionResult<string>.Failed("Email is associated with a user", StatusCodes.Status400BadRequest);
            }

            var user = new User(newUser.FirstName, newUser.LastName, newUser.Email, authType, role);


            if (!string.IsNullOrEmpty(newUser.Password) || !string.IsNullOrWhiteSpace(newUser.Password))
            {
                var hashedPassword = passwordManager.HashPassword(newUser.Password, out string salt);

                user.SetPassword(hashedPassword, salt);
            }

            if (authType != AuthType.local)
            {
                user.ConfirmEmailVerification();
            }

            var result = await userService.CreateUserAsync(user);

            var recoveryCode = new RecoveryCode(user.Id);
            var response = await recoveryCodeRepository.Add(recoveryCode);

            if (!result || !response)
                return ActionResult<string>.Failed("Failed to remove user token", StatusCodes.Status400BadRequest);

            if (authType != AuthType.local)
            {
                return ActionResult<string>.SuccessfulOperation("Successfully registered");
            }

            var claims = new Dictionary<string, object>() { { "id", user.Id.ToString("N") },
                { ClaimTypes.Email,user.Email} };

            var emailConfirmationToken = tokenProvider.GenerateToken(claims, 30);

            var substitutions = new Dictionary<string, string>()
                {
                    {"[Token]" , emailConfirmationToken },
                    {"[Name]" , $"{ user.FirstName} {user.LastName}" }
                };

            _notificationService.SendNotification(new Email(user.Email, "Confirm email"),
            EmailTemplate.EmailConfirmationNotification, ContentType.html, substitutions);

            return ActionResult<string>.SuccessfulOperation(emailConfirmationToken);
        }

        public async Task<ActionResult<UserDTO>> LoginUser(LoginDTO login, AuthType authType)
        {
            var authenticatedUser = await userService.AuthenticateUsingEmailAsync(login.Email, login.Password);

            if (authenticatedUser == null)
            {
                return ActionResult<UserDTO>.Failed("Invalid email/password", StatusCodes.Status400BadRequest);
            }

            if (authenticatedUser.LockoutEnabled)
            {
                return ActionResult<UserDTO>.Failed("Account has been disable. Please contact admin");

            }

            if (!authenticatedUser.EmailConfirmed)
            {
                var claims = new Dictionary<string, object>() { { "id", authenticatedUser.Id.ToString("N") },
                    { ClaimTypes.Email,authenticatedUser.Email} , { ClaimTypes.Role, authenticatedUser.Role.ToString()} };
                var emailConfirmationToken = tokenProvider.GenerateToken(claims, 30);

                var substitutions = new Dictionary<string, string>()
                    {
                    {"[Token]" , emailConfirmationToken },
                    {"[Name]" , $"{ authenticatedUser.FirstName} {authenticatedUser.LastName}" }
                    };

                _notificationService.SendNotification(new Email(authenticatedUser.Email, "Confirm email"),
                    EmailTemplate.EmailConfirmationNotification, ContentType.html, substitutions);
                return ActionResult<UserDTO>.Failed("Confirm email", StatusCodes.Status400BadRequest);
            }

            var isFirstTimeLogin = authenticatedUser.RefreshToken == null;

            if (authenticatedUser.RefreshToken != null)
            {
                await refreshTokenRepository.Remove(authenticatedUser.RefreshToken);
            }

            var accessToken = tokenProvider.GenerateToken(new Dictionary<string, object>() { { "id", authenticatedUser.Id.ToString("N") },
                { ClaimTypes.Email,authenticatedUser.Email} , { ClaimTypes.Role,authenticatedUser.Role.ToString()} }, 10);

            var refreshToken = tokenProvider.GenerateToken();

            var token = new RefreshToken(refreshToken, authenticatedUser.Id, DateTime.UtcNow.AddMonths(1));

            var tokenResponse = await refreshTokenRepository.Add(token);

            if (!tokenResponse)
                return ActionResult<UserDTO>.Failed("Failed to login");

            return ActionResult<UserDTO>.SuccessfulOperation(new UserDTO(authenticatedUser.Id, authenticatedUser.FirstName,
                authenticatedUser.LastName, authenticatedUser.Email, isFirstTimeLogin ? authenticatedUser.RecoveryCode?.Codes : null, refreshToken, accessToken));
        }

        public async Task<ActionResult<string>> GenerateAccessToken(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return ActionResult<string>.Failed("Invalid refresh token", StatusCodes.Status400BadRequest);
            }

            var userId = await refreshTokenRepository.GetUserByRefreshToken(refreshToken);

            var user = await userRepository.GetUser(userId);

            if (user == null)
            {
                return ActionResult<string>.Failed("Invalid refresh token", StatusCodes.Status404NotFound);
            }

            var accessToken = tokenProvider.GenerateToken(new Dictionary<string, object>() { { "id", user.Id.ToString("N") },
                { ClaimTypes.Email, user.Email}, { ClaimTypes.Role , user.Role.ToString()} }, 10);

            return ActionResult<string>.SuccessfulOperation(accessToken);
        }

        public async Task<ActionResult<string>> GenerateAccessToken(Guid userId, string email)
        {
            if (userId == Guid.Empty)
                return ActionResult<string>.Failed("Invalid user id", StatusCodes.Status400BadRequest);

            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            {
                return ActionResult<string>.Failed("Invalid email", StatusCodes.Status400BadRequest);
            }

            var user = await userRepository.GetUser(userId);

            if (user == null)
            {
                return ActionResult<string>.Failed("Invalid refresh token", StatusCodes.Status400BadRequest);
            }

            var accessToken = tokenProvider.GenerateToken(new Dictionary<string, object>() { { "id", user.Id.ToString("N") },
                { ClaimTypes.Email, user.Email} ,{ ClaimTypes.Role , user.Role.ToString()} }, 10);

            return ActionResult<string>.SuccessfulOperation(accessToken);
        }

        public async Task<ActionResult> ConfirmEmail(string verificationToken)
        {
            if (string.IsNullOrEmpty(verificationToken) || string.IsNullOrWhiteSpace(verificationToken))
            {
                return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);
            }

            var claims = tokenProvider.ReadToken(verificationToken, false, "id", ClaimTypes.Email);

            if (claims == null)
            {
                return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);
            }

            var id = new Guid(claims.GetValueOrDefault("id").ToString());

            var user = await userRepository.GetUser(id);

            if (user == null)
            {
                return ActionResult.Failed("Invalid token", StatusCodes.Status400BadRequest);
            }

            if (user.EmailConfirmed)
            {
                return ActionResult.Failed("Email has been verified");
            }


            user.ConfirmEmailVerification();


            var result = await userRepository.Update(user);
            if (!result)
            {
                return ActionResult.Failed("Failed to confirm email");
            }

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult<bool>> IsEmailVerified(Guid userId)
        {

            var user = await userRepository.GetUser(userId);

            if (user == null)
            {
                return ActionResult<bool>.Failed("Invalid token", StatusCodes.Status400BadRequest);
            }

            return ActionResult<bool>.SuccessfulOperation(user.EmailConfirmed);
        }

        public async Task<ActionResult<string>> LoginUserPasswordless(string email)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
            {
                return ActionResult<string>.Failed("Invalid email", StatusCodes.Status400BadRequest);
            }

            var user = await userRepository.GetUserByEmail(email);
            if (user == null)
            {
                return ActionResult<string>.Failed("Invalid email", StatusCodes.Status400BadRequest);
            }

            if (user.LockoutEnabled)
            {
                return ActionResult<string>.Failed("Account has been locked. Please contact admin", StatusCodes.Status400BadRequest);
            }

            if (!user.EmailConfirmed)
            {
                return ActionResult<string>.Failed("Confirm email", StatusCodes.Status400BadRequest);
            }


            if (user.AuthenticationType != AuthType.local)
            {
                return ActionResult<string>.Failed("Invalid email", StatusCodes.Status400BadRequest);
            }

            var token = tokenProvider.GenerateToken(new Dictionary<string, object>() { { ClaimTypes.Email, user.Email } }, 10);

            var substitutions = new Dictionary<string, string>()
                    {
                    {"[Token]" , token }
                    };


            _notificationService.SendNotification(new Email(user.Email, "Login Passwordless"),
                EmailTemplate.PasswordlessNotification, ContentType.html, substitutions);

            return ActionResult<string>.SuccessfulOperation(token);

        }

        public async Task<ActionResult<UserDTO>> VerifyPasswordlessToken(string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
            {
                return ActionResult<UserDTO>.Failed("Invalid token", StatusCodes.Status400BadRequest);
            }

            var claims = tokenProvider.ReadToken(token, false, ClaimTypes.Email);

            if (claims.Count == 0 || claims.Count >= 2)
            {
                return ActionResult<UserDTO>.Failed("Invalid token", StatusCodes.Status400BadRequest);
            }

            var user = await userRepository.GetUserByEmail(claims.GetValueOrDefault(ClaimTypes.Email).ToString());

            if (user == null)
            {
                return ActionResult<UserDTO>.Failed("Failed to login", StatusCodes.Status400BadRequest);
            }

            if (user.LockoutEnabled)
            {
                return ActionResult<UserDTO>.Failed("Account has been locked. Please contact admin", StatusCodes.Status400BadRequest);
            }

            var isFirstTimeLogin = user.RefreshToken == null;

            if (user.RefreshToken != null)
            {

                await refreshTokenRepository.Remove(user.RefreshToken);
            }

            var accessToken = tokenProvider.GenerateToken(new Dictionary<string, object>() { { "id", user.Id.ToString("N") },
                    { ClaimTypes.Email, user.Email} , { ClaimTypes.Role, user.Role.ToString()} }, 10);

            var refreshToken = tokenProvider.GenerateToken();

            var newRefreshToken = new RefreshToken(refreshToken, user.Id, DateTime.UtcNow.AddMonths(3));
            var tokenResponse = await refreshTokenRepository.Add(newRefreshToken);

            if (!tokenResponse)
                return ActionResult<UserDTO>.Failed("Failed to login", StatusCodes.Status400BadRequest);


            var authenticatedUser = new UserDTO(user.Id, user.FirstName, user.LastName, user.Email, 
                isFirstTimeLogin ? user.RecoveryCode?.Codes : null, refreshToken, accessToken);

            return ActionResult<UserDTO>.SuccessfulOperation(authenticatedUser);
        }

        public async Task<ActionResult<UserDTO>> LoginUserByRecoveryCodes(string[] recoveryCodes)
        {
            if (recoveryCodes.Length == 0)
                return ActionResult<UserDTO>.Failed("Invalid recovery codes", StatusCodes.Status400BadRequest);

            var recoveryCode = string.Join(',', recoveryCodes);

            var currentCode = await recoveryCodeRepository.GetUserIdByRecoveryCodes(recoveryCode);

            if (currentCode == null)
            {
                return ActionResult<UserDTO>.Failed("Failed to login", StatusCodes.Status400BadRequest);
            }

            var user = currentCode.User;

            await recoveryCodeRepository.Remove(currentCode);

            var newRecoveryCode = new RecoveryCode(user.Id);

            var codeResponse = await recoveryCodeRepository.Add(newRecoveryCode);

            if (!codeResponse)
            {
                return ActionResult<UserDTO>.Failed("Failed to login", StatusCodes.Status400BadRequest);
            }
            if (user.LockoutEnabled)
            {
                return ActionResult<UserDTO>.Failed("Account has been locked. Please contact admin", StatusCodes.Status400BadRequest);
            }

            var isFirstTimeLogin = user.RefreshToken == null;

            if (user.RefreshToken != null)
            {
                await refreshTokenRepository.Remove(user.RefreshToken);
            }

            var accessToken = tokenProvider.GenerateToken(new Dictionary<string, object>() { { "id", user.Id.ToString("N") },
                    { ClaimTypes.Email, user.Email} , { ClaimTypes.Role,user.Role.ToString()} }, 10);

            var refreshToken = tokenProvider.GenerateToken();

            var newRefreshToken = new RefreshToken(refreshToken, user.Id, DateTime.UtcNow.AddMonths(3));

            var tokenResponse = await refreshTokenRepository.Add(newRefreshToken);

            if (!tokenResponse)
                return ActionResult<UserDTO>.Failed("Failed to login", StatusCodes.Status400BadRequest);

            var authenticatedUser = new UserDTO(user.Id, user.FirstName, user.LastName, user.Email, newRecoveryCode.Codes,refreshToken, accessToken);

            return ActionResult<UserDTO>.SuccessfulOperation(authenticatedUser);

        }



        public async Task<ActionResult<string[]>> GenerateRecoveryCodes(Guid userId)
        {
            var currentUser = await userRepository.GetUser(userId);

            if (currentUser == null)
            {
                return ActionResult<string[]>.Failed("Invalid user", StatusCodes.Status404NotFound);
            }

            var recoveryCode = new RecoveryCode(userId);

            if (currentUser.RecoveryCode != null)
            {

                await recoveryCodeRepository.Remove(currentUser.RecoveryCode);
            }

            var commitStatus = await recoveryCodeRepository.Add(recoveryCode);

            if (!commitStatus) return ActionResult<string[]>.Failed("Failed to generate recovery codes", StatusCodes.Status400BadRequest);

            return ActionResult<string[]>.SuccessfulOperation(recoveryCode.Codes.Split(','));
        }



        /// <summary>
        /// Two factor authentication secret key
        /// </summary>
        public string key;
        /// <summary>
        /// A sms service used to send message via sms
        /// </summary>
        public ISmsService smsService;

        public IUserRepository userRepository;

        public ITokenProvider tokenProvider;

        public IMfaService mfaService;

        /// <summary>
        ///  Handles the cloud storage
        /// </summary>
        public IBlobService blobService;

        public IRefreshTokenRepository refreshTokenRepository;

        public IPasswordManager passwordManager;

        /// <summary>
        /// Secret to generate multifactor token
        /// </summary>
        public string loginSecret;

        public INotificationService _notificationService;

        public IRecoveryCodeRepository recoveryCodeRepository;

        public IUserService<User> userService;
    }
}
