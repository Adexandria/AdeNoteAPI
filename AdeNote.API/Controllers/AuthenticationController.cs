﻿using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services.Authentication;
using AdeNote.Infrastructure.Services.Notification;
using AdeNote.Infrastructure.Services.UserSettings;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.EmailSettings;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Infrastructure.Utilities.ValidationAttributes;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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
        public AuthenticationController(IUserIdentity userIdentity,
            IAuthService authService, INotificationService notificationService, IUserService userService) : base(userIdentity)
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
            if(response.Data != "Successfully registered" && response.IsSuccessful)
            {
                return Ok(new
                {
                    emailConfirmationToken = response.Data
                });
            }
            return response.Response();
        }

        /// <summary>
        /// Sign ups a new user
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///             
        ///             POST /authentication/admin/sign-up
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
        [HttpPost("admin/sign-up")]
        [Authorize("Owner")]
        public async Task<IActionResult> AdminSignUp(CreateUserDTO newUser)
        {
            var response = await _authService.SignUser(newUser, role: Role.Admin);
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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<LoginDTO>), StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("login/passwordless")]
        public async Task<IActionResult> Login([FromQuery][Required(ErrorMessage = "Invalid email")] string email)
        {
            var response = await _authService.LoginUserPasswordless(email);
            return response.Response();
        }


        /// <summary>
        /// Logins user using recovery codes
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///             
        ///             POST /authentication/login/recovery-codes
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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<UserDTO>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<UserDTO>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("login/recovery-codes")]
        public async Task<IActionResult> LoginByRecoveryCodes([ValidCollection("Invalid recovery codes")] string[] codes)
        {
            var response = await _authService.LoginUserByRecoveryCodes(codes);
            if (response.NotSuccessful)
                return response.Response();

            var resultResponse = _authService.IsAuthenticatorEnabled(response.Data.Email).Result;

            if (resultResponse.Data != MFAType.none.ToString())
            {
                var tokenResponse = _authService.GenerateMFAToken(response.Data.UserId, response.Data.Email, response.Data.RefreshToken);

                AddToCookie("Multi-FactorToken", tokenResponse.Data, DateTime.UtcNow.AddMinutes(8));

                return Infrastructure
                    .Utilities.
                    ActionResult<string>
                    .SuccessfulOperation($"Proceed to enter otp from {resultResponse.Data} authenticator")
                    .Response();
            }

            SendNotification(response.Data.Email);

            AddToCookie("AdeNote-RefreshToken", response.Data.RefreshToken, DateTime.UtcNow.AddMonths(2));

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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("login/passwordless/verify-token")]
        public async Task<IActionResult> VerifyPasswordlessToken([FromQuery] string token, CancellationToken cancellationToken)
        {
            var response = await _authService.VerifyPasswordlessToken(token);

            if (response.NotSuccessful)
                return response.Response();

            var resultResponse = _authService.IsAuthenticatorEnabled(response.Data.Email).Result;

            if (resultResponse.Data != MFAType.none.ToString())
            {
                var tokenResponse = _authService.GenerateMFAToken(response.Data.UserId, response.Data.Email, response.Data.RefreshToken);

                AddToCookie("Multi-FactorToken", tokenResponse.Data, DateTime.UtcNow.AddMinutes(8));

                return Infrastructure
                  .Utilities.
                  ActionResult<string>
                  .SuccessfulOperation($"Proceed to enter otp from {resultResponse.Data} authenticator")
                  .Response();
            }

            SendNotification(response.Data.Email);

            AddToCookie("AdeNote-RefreshToken", response.Data.RefreshToken, DateTime.UtcNow.AddMonths(2));

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
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<LoginDTO>), StatusCodes.Status200OK)]
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
                var response = await _authService.SignUser(new CreateUserDTO() { Email = userEmail, FirstName = names[0], LastName = names[1] }, AuthType.microsoft);

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
        [Authorize("User")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDTO changePassword)
        {
            var response = await _userService.UpdateUserPassword(CurrentUser,
                changePassword.CurrentPassword, changePassword.Password);
            return response.Response();
        }

        /// <summary>
        /// Reset password using reset password token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /authentication/reset-password
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
        public async Task<IActionResult> ResetPassword([FromBody][Password] string password, [FromQuery] string token)
        {
            var tokenResponse = _authService.VerifyResetToken(token);

            if (tokenResponse.NotSuccessful)
                return tokenResponse.Response();

            var response = await _userService.ResetUserPassword(CurrentUser, password);

            return response.Response();
        }

        /// <summary>
        /// Generate reset password token
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///     
        ///             POST /authentication/reset-password-token
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
        [HttpPost("reset-password-token")]
        public async Task<IActionResult> GenerateResetPasswordToken([FromBody] string email, CancellationToken cancellationToken)
        {
            var userResponse = await _userService.GetUser(email);

            if(userResponse.NotSuccessful)
                return userResponse.Response();

            var response = await _authService.GenerateResetPasswordToken(userResponse.Data.Id, email);

            return response.Response();
        }
        /// <summary>
        /// Gets access token using refresh token
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /authentication/access-token
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
        [HttpPost("access-token")]
        public async Task<IActionResult> GetAccessToken(CancellationToken cancellationToken)
        {
            var refreshToken = Request.Cookies["AdeNote-RefreshToken"];

            var revokedResponse = await _authService.IsTokenRevoked(refreshToken);
            if (revokedResponse.NotSuccessful)
                return revokedResponse.Response();


            var response = await _authService.GenerateAccessToken(refreshToken);
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
        public async Task<IActionResult> Verifyemail([Required]string verificationToken)
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
        public async Task<IActionResult> VerifySmsOTP([Required] string otp)
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
        ///                 POST /authentication/two-factor-authentication/google/verify-code
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
        [HttpPost("two-factor-authentication/google/verify-code")]
        public async Task<IActionResult> VerifyTOTP([Required] string totp)
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
        ///             GET /authentication/two-factor-authentication/google/qr-code
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
        [Authorize("User")]
        [HttpGet("two-factor-authentication/google/qr-code")]
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

            if (response.IsSuccessful)
            {
                Response.Cookies.Delete("AdeNote-RefreshToken");
            }

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
        [Authorize("User")]
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
        ///                 DELETE authentication/two-factor-authentication/recovery
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
        public async Task<IActionResult> GenerateMFARemovalToken([Required] string email)
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
        ///                 DELETE authentication/two-factor-authentication/verify-recovery-token
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
        [HttpDelete("two-factor-authentication/verify-recovery-token")]
        public async Task<IActionResult> RemoveAuthenicator([FromBody] [Required(AllowEmptyStrings = false)] string token)
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
                 { "[Date]", DateTime.UtcNow.ToLongDateString() },
                {"[Time]",DateTime.UtcNow.ToLongTimeString() }
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
                var tokenResponse = _authService.GenerateMFAToken(loginResponse.Data.UserId, login.Email, loginResponse.Data.RefreshToken);

                AddToCookie("Multi-FactorToken", tokenResponse.Data, DateTime.UtcNow.AddMinutes(8));

                return Infrastructure
                    .Utilities.
                     ActionResult<string>
                     .SuccessfulOperation("$Proceed to enter otp from {resultResponse.Data} authenticator")
                    .Response();
            }

            SendNotification(loginResponse.Data.Email);

            AddToCookie("AdeNote-RefreshToken", loginResponse.Data.RefreshToken, DateTime.UtcNow.AddMonths(2));

            return Ok(new 
            {
                userId = loginResponse.Data.UserId,
                firstName = loginResponse.Data.FirstName,
                lastName = loginResponse.Data.LastName,
                email = loginResponse.Data.Email,
                accessToken = loginResponse.Data.AccessToken,
                recoveryCodes = loginResponse.Data.Codes
            });
        }
    }
}
