using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdeNote.Controllers
{
    /// <summary>
    /// This handles the authentication.
    /// 
    /// 
    /// Supports version 1.0
    /// </summary>
    [Route("api/v{version:apiVersion}/authentication")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private readonly IUserService _userService;
        /// <summary>
        /// This is the constructor
        /// </summary>
        /// <param name="authService">An authentication service </param>
        /// <param name="notificationService">Handles Notification</param>
        /// <param name="userIdentity">An interface that interacts with the user. This fetches the current user details</param>
        /// <param name="userService">An interface that manages users</param>
        public AuthenticationController( IUserIdentity userIdentity,
            IAuthService authService, INotificationService notificationService,IUserService userService) : base(userIdentity)
        {
            _authService = authService;
            _notificationService = notificationService;
            _userService = userService;
        }

        /// <summary>
        /// Sign ups a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///             
        ///             POST /authentication/sign-up
        ///             {
        ///                
        ///                     "name": "string",
        ///                      "email": "string",
        ///                      "password": "string",
        ///                      "confirmPassword": "string"
        ///
        ///             }
        ///             
        /// </remarks>
        /// <response code ="200"> Returns if user was created</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>A name and email</returns>
        [AllowAnonymous] 
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<CreateUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(CreateUserDTO newUser)
        {
            var response = await _authService.SignUser(newUser);
            return response.Response();
        }


        /// <summary>
        /// Logins user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///             
        ///             POST /authentication/login
        ///             {
        ///                "email": "string",
        ///                "password": "string"
        ///             }
        ///  </remarks>
        /// <response code ="200"> Returns if logged in successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>A name and email</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionTokenResult<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public IActionResult Login(LoginDTO login)
        {
            return LoginUser(login, AuthType.local);
        }


        /// <summary>
        /// Logins user using passwordless method
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///             
        ///             POST /authentication/login/passwordless?email="email@gmail.com
        ///             
        ///  </remarks>
        /// <response code ="200"> Returns if logged in successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>A name and email</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionTokenResult<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("login/passwordless")]
        public async Task<IActionResult> Login([FromQuery]string email)
        {
           var response = await _authService.LoginUser(email);
           return response.Response();
        }


        /// <summary>
        /// Logins user using passwordless method
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///             
        ///             POST /authentication/login/passwordless/verify-token
        ///             
        ///  </remarks>
        /// <response code ="200"> Returns if logged in successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>A name and email</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionTokenResult<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("login/passwordless/verify-token")]
        public async Task<IActionResult> VerifyPasswordlessToken([FromQuery] string token)
        {
            var response = await _authService.VerifyPasswordlessToken(token);

            if (response.NotSuccessful)
                return response.Response();

            var resultResponse = _authService.IsAuthenticatorEnabled(response.Data.Email).Result;

            if (resultResponse.Data != MFAType.none.ToString())
            {
                var tokenResponse = _authService.GenerateMFAToken(response.Data.UserId, response.Data.Email, response.RefreshToken);

                AddToCookie("Multi-FactorToken", tokenResponse.Data, DateTime.UtcNow.AddMinutes(8));

                return Ok($"Proceed to enter otp from {resultResponse.Data} authenticator");
            }

            /*  SendNotification(response.Data.Email);*/

            AddToCookie("AdeNote-RefreshToken", response.RefreshToken, DateTime.UtcNow.AddMonths(2));

            return response.Response();
        }


        /// <summary>
        /// Logins user
        /// </summary>
        /// <response code ="200"> Returns if logged in successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>A name and email</returns>
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(ActionTokenResult<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("sso")]
        [Authorize("sso")]
        public async Task<IActionResult> LoginSSO()
        {
            var userEmail = HttpContext.User.FindFirstValue(ClaimTypes.Email);

            var name = HttpContext.User.FindFirstValue("name");

            if (string.IsNullOrEmpty(name))
            {
                return Infrastructure.Utilities.ActionResult.Failed("Invalid token").Response();
            }

            var names = name.Split(' ');

            var isExist = await _userService.IsUserExist(userEmail);

            if (isExist.NotSuccessful)
            {
                var response = await _authService.SignUser(new CreateUserDTO() { Email = userEmail , FirstName = names[0], LastName= names[1]},AuthType.microsoft);

                if (response.NotSuccessful)
                    return response.Response();
            }
            var loginUser = new LoginDTO()
            {
                Email = userEmail
            };


            return LoginUser(loginUser, AuthType.microsoft);
        }

        /// <summary>
        /// Change password
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /authentication/change-password
        /// </remarks>
        /// <param name="changePassword">A model to change password</param>
        /// <response code ="200"> Returns if password was changed successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response> 
        /// <response code ="404"> Returns if user can't be found</response> 
        /// <returns>Action result</returns>
        [Consumes("application/json")]
        [Produces("application/json")]
        [Authorize]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePassword)
        {
            var response = await _userService.UpdateUserPassword(CurrentUser, changePassword.CurrentPassword, changePassword.Password);
            return response.Response();
        }

        /// <summary>
        /// Reset password
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /authentication/password/verify-token
        /// </remarks>
        /// <param name="password">New password</param>
        /// <param name="token">A token to allow user to reset password</param>
        /// <response code ="200"> Returns if password was changed successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response> 
        /// <response code ="404"> Returns if user can't be found</response> 
        /// <returns>Action result</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody]string password,[FromQuery] string token)
        {
            var tokenResponse = _authService.VerifyResetToken(token);
            if (tokenResponse.NotSuccessful)
                return tokenResponse.Response();
            var response = await _userService.ResetUserPassword(CurrentUser,password);
            return response.Response();
        }

        /// <summary>
        /// Generate token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     
        ///             POST /authentication/password/send-token
        /// </remarks>
        /// <param name="email">email of the user</param>
        /// <response code ="200"> Returns if token was generated</response>
        /// <response code ="404"> Returns if user can't be found</response>
        /// <returns>Action result</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [HttpPost("generate-token")]
        public async Task<IActionResult> GenerateResetToken(string email)
        {
            var userResponse = await _userService.GetUser(email);
            if(userResponse.NotSuccessful)
                return userResponse.Response();
            var response = await _authService.GenerateResetToken(userResponse.Data.Id, email);
            return response.Response();
        }
        /// <summary>
        /// Gets access token using refresh token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /authentication/token
        /// </remarks>
        /// <returns>An access token</returns>
        /// <response code ="200"> Returns if logged in successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <response code ="404"> Returns if user can't be found</response>
        /// <returns>Access token</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("token")]
        public async Task<IActionResult> GetAccessToken()
        {
            var refreshToken = Request.Cookies["AdeNote-RefreshToken"];

            var revokedResponse = await _authService.IsTokenRevoked(refreshToken);
            if (revokedResponse.NotSuccessful)
                return revokedResponse.Response();


            var response = await _authService.GenerateAccessToken(refreshToken);
            return response.Response();
        }

        /// <summary>
        /// Sets up two factor authentication using google authenticator app
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /authentication/two-factor-authentication/app
        ///             
        /// </remarks>
        /// <response code ="200"> Returns if two factor authenticator was successfully set up</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>Authenticator key</returns>
        [Produces("application/json")]
        [Authorize]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<AuthenticatorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("two-factor-authentication/app")]
        public async Task<IActionResult> SetUpGoogleAuthenticator()
        {
            var resultResponse = await _authService.IsAuthenticatorEnabled(CurrentUser, MFAType.google);

            if (resultResponse.IsSuccessful)
                return Infrastructure.Utilities.ActionResult.Failed("User has set up two factor authentication", StatusCodes.Status400BadRequest).Response();

            var response = await _authService.SetAuthenticator(CurrentUser, CurrentEmail);

            return response.Response();
        }

        /// <summary>
        /// Sets up two factor authentication using sms
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /authentication/two-factor-authentication/sms
        ///             
        /// </remarks>
        /// <response code ="200"> Returns if two factor authenticator was successfully set up</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Authorize]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("two-factor-authentication/sms")]
        public async Task<IActionResult> SetUpSmsAuthenticator()
        {
            var resultResponse = await _authService.IsAuthenticatorEnabled(CurrentUser, MFAType.sms);

            if (resultResponse.IsSuccessful)
                return Infrastructure.Utilities.ActionResult.Failed("User has set up two factor authentication", StatusCodes.Status400BadRequest).Response();

            var verificationResponse = await _authService.IsPhoneNumberVerified(CurrentUser);

            if (verificationResponse.NotSuccessful)
                return verificationResponse.Response();

            var response = await _authService.SetSmsAuthenticator(CurrentUser);

            return response.Response();
        }

        /// <summary>
        /// Adds phone number
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /authentication/phonenumber
        ///             
        /// </remarks>
        /// <param name="phoneNumber">User phone number</param>
        /// <response code ="200"> Returns if phone number was added/response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Authorize]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("phonenumber")]
        public async Task<IActionResult> AddPhoneNumber(string phoneNumber)
        {
            var response = await _authService.SetPhoneNumber(CurrentUser,phoneNumber);

            return response.Response();
        }

        /// <summary>
        /// Verifies phone number
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /authentication/verify-phonenumber
        ///             
        /// </remarks>
        /// <param name="verificationCode">Verification code</param>
        /// <response code ="200"> Returns if phone number was verified</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Authorize]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("verify-phonenumber")]
        public async Task<IActionResult> VerifyPhoneNumber(string verificationCode)
        {
            var resultResponse = await _authService.IsPhoneNumberVerified(CurrentUser);

            if(resultResponse.IsSuccessful)
                return Infrastructure.Utilities
                    .ActionResult.Failed("Phone number is already verified",StatusCodes.Status400BadRequest)
                    .Response();

            var response = await _authService.VerifyPhoneNumber(CurrentUser,verificationCode);

            return response.Response();
        }

        /// <summary>
        /// Verifies email
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /authentication/verify-email
        ///             
        /// </remarks>
        /// <param name="verificationToken">Verification code</param>
        /// <response code ="200"> Returns if phone number was verified</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("verify-email")]
        [AllowAnonymous]
        public async Task<IActionResult> Verifyemail(string verificationToken)
        {

            var response = await _authService.ConfirmEmail(verificationToken);

            return response.Response();
        }





        /// <summary>
        /// Sends one time password via sms
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /two-factor-authentication/sms/send-code
        ///             
        /// </remarks>
        /// <response code ="200"> Returns if code was sent</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("two-factor-authentication/sms/send-code")]
        public async Task<IActionResult> SendOTP()
        {
            var token = Request.Cookies["Multi-FactorToken"];

            var detailsResponse = _authService.ReadDetailsFromToken(token);
            if (detailsResponse.NotSuccessful)
                return detailsResponse.Response();

            var resultResponse = await _authService.IsAuthenticatorEnabled(detailsResponse.Data.UserId, MFAType.sms);

            if (resultResponse.NotSuccessful)
                return Infrastructure.Utilities
                    .ActionResult.Failed("User has not set up two factor authentication", StatusCodes.Status400BadRequest)
                    .Response();

            var verificationResponse = await _authService.IsPhoneNumberVerified(detailsResponse.Data.UserId);

            if (verificationResponse.NotSuccessful)
                return Infrastructure.Utilities.ActionResult.Failed("Phone number has not been verified", StatusCodes.Status400BadRequest).Response();

            var response = await _authService.SendSmsOTP(detailsResponse.Data.UserId,detailsResponse.Data.Email);

            return response.Response();
        }


        /// <summary>
        /// Verifies sms code
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /two-factor-authentication/sms/verify-code
        ///             
        /// </remarks>
        /// <response code ="200"> Returns if verification code was successfully verified</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>Access token</returns>
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("two-factor-authentication/sms/verify-code")]
        public async Task<IActionResult> VerifySmsOTP(string otp)
        {
            var token = Request.Cookies["Multi-FactorToken"];

            var detailsResponse = _authService.ReadDetailsFromToken(token);
            if (detailsResponse.NotSuccessful)
                return detailsResponse.Response();

            var resultResponse = await _authService.IsAuthenticatorEnabled(detailsResponse.Data.UserId, MFAType.sms);

            if (resultResponse.NotSuccessful)
                return Infrastructure.Utilities
                    .ActionResult.Failed("User has not set up two factor authentication", StatusCodes.Status400BadRequest)
                    .Response();

            var verificationResponse = await _authService.IsPhoneNumberVerified(detailsResponse.Data.UserId);

            if (verificationResponse.NotSuccessful)
                return Infrastructure.Utilities
                    .ActionResult.Failed("Phone number has not been verified", StatusCodes.Status400BadRequest)
                    .Response();

            var response =  _authService.VerifyAuthenticatorOTP(detailsResponse.Data.Email, otp);

            if (response.NotSuccessful)
                return response.Response();

            var accessToken = await _authService.GenerateAccessToken(detailsResponse.Data.UserId, detailsResponse.Data.Email);

            AddToCookie("AdeNote-RefreshToken", detailsResponse.Data.RefreshToken, DateTime.UtcNow.AddMonths(2));

            SendNotification(detailsResponse.Data.Email);

            return Infrastructure.Utilities.ActionResult<string>.SuccessfulOperation(accessToken.Data).Response();
        }

        /// <summary>
        /// Verifies time based One time password
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///                 POST /authentication/two-factor-authentication/app/verify-code
        /// </remarks>
        /// <param name="totp">Timed based one time password</param>
        /// <response code ="200"> Returns if one time password was verified correctly</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response> 
        /// <returns>Access token</returns>
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("two-factor-authentication/app/verify-code")]
        public async Task<IActionResult> VerifyTOTP(string totp)
        {
            var token = Request.Cookies["Multi-FactorToken"];

            var detailsResponse = _authService.ReadDetailsFromToken(token);
            if(detailsResponse.NotSuccessful)
                return detailsResponse.Response();

            var response = _authService.VerifyAuthenticatorOTP(detailsResponse.Data.Email, totp);
            if(response.NotSuccessful)
                return response.Response();

            var accessToken = await _authService.GenerateAccessToken(detailsResponse.Data.UserId, detailsResponse.Data.Email);

            SendNotification(detailsResponse.Data.Email);

            AddToCookie("AdeNote-RefreshToken", detailsResponse.Data.RefreshToken, DateTime.UtcNow.AddMonths(2));

            return Ok(accessToken);
        }


        /// <summary>
        /// Gets existing qr code
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///             GET /authentication/two-factor-authentication/app/qr-code
        /// </remarks>
        /// <response code ="200"> Returns if google two factor authenticator was enabled for the user</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response> 
        /// <returns>Qr ucode url</returns>
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpGet("two-factor-authentication/app/qr-code")]
        public async Task<IActionResult> GetAuthenticatorQRCode()
        {
            var response = await _authService.GetUserQrCode(CurrentUser);
            return response.Response();
        }


        /// <summary>
        /// Signs out user
        /// </summary>
        /// <remarks>
        /// Sample request
        /// 
        ///             POST /authentication/sign-out
        /// </remarks>
        /// <response code ="200"> Returns if user was logged out successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response> 
        /// <returns>Action result</returns>
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("sign-out")]
        public async Task<IActionResult> LogOut()
        {
            var refreshToken = Request.Cookies["AdeNote-RefreshToken"];

            var response = await _authService.RevokeRefreshToken(CurrentUser, refreshToken);

            Response.Cookies.Delete("AdeNote-RefreshToken");

            return response.Response();
        }

        /// <summary>
        /// Removes google or sms authenticator 
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///                 DELETE authentication/two-factor-authentication
        /// </remarks>
        /// <returns>Action result</returns>
        /// <response code ="200"> Returns if multifactor authentication was disabled successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response> 
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [Authorize]
        [HttpDelete("two-factor-authentication")]
        public async Task<IActionResult> RemoveAuthenicator()
        {
            var response = await _authService.DisableUserMFA(CurrentUser);
            return response.Response();
        }


        /// <summary>
        /// Generates token to removes google or sms authenticator 
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///                 DELETE authentication/two-factor-authentication
        /// </remarks>
        /// <returns>Action result</returns>
        /// <response code ="200"> Returns if multifactor authentication was disabled successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response> 
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpGet("two-factor-authentication/recovery")]
        public async Task<IActionResult> GenerateMFARemovalToken(string email)
        {
            var response = await _authService.GenerateMFAToken(email);
            return response.Response();
        }



        /// <summary>
        /// Removes google or sms authenticator using token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///                 DELETE authentication/two-factor-authentication/verify-token
        /// </remarks>
        /// <returns>Action result</returns>
        /// <response code ="200"> Returns if multifactor authentication was disabled successful</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response> 
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpDelete("two-factor-authentication/verify-token")]
        public async Task<IActionResult> RemoveAuthenicator([FromBody] string token)
        {
            var response = await _authService.DisableUserMFA(token);
            return response.Response();
        }




        /// <summary>
        /// Add data to cookie
        /// </summary>
        /// <param name="name">Name of the cookie</param>
        /// <param name="data">Data to display</param>
        /// <param name="time">Time left for the cookie to expire</param>
        [NonAction]
        private void AddToCookie(string name,string data, DateTime time)
        {
            Response.Cookies.Append(name, data,
                  new CookieOptions()
                  {
                      Expires = time,
                      Secure = true,
                      SameSite = SameSiteMode.None,
                      HttpOnly = true
                  });
        }

        [NonAction]
        private void SendNotification(string email)
        {
            var substitutions = new Dictionary<string, string>
            {
                 { "[Date]", DateTime.Now.ToLongDateString() },
                {"[Time]",DateTime.Now.ToLongTimeString() }
            };
            _notificationService.SendNotification(new Email(email, "New login to AdeNote"),
            EmailTemplate.LoginNotification, ContentType.html, substitutions);

        }

        [NonAction]
        private IActionResult LoginUser(LoginDTO login, AuthType authType)
        {
            var loginResponse = _authService.LoginUser(login, authType).Result;

            if (loginResponse.NotSuccessful)
                return loginResponse.Response();

            var resultResponse = _authService.IsAuthenticatorEnabled(login.Email).Result;

            if (resultResponse.Data != MFAType.none.ToString())
            {
                var tokenResponse = _authService.GenerateMFAToken(loginResponse.Data.UserId, login.Email, loginResponse.RefreshToken);

                AddToCookie("Multi-FactorToken", tokenResponse.Data, DateTime.UtcNow.AddMinutes(8));

                return Ok($"Proceed to enter otp from {resultResponse.Data} authenticator");
            }

           // SendNotification(loginResponse.Data.Email);

            AddToCookie("AdeNote-RefreshToken", loginResponse.RefreshToken, DateTime.UtcNow.AddMonths(2));

            return loginResponse.Response();
        }
    }
}
