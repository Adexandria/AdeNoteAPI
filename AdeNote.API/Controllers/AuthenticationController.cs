using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using Autofac;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TasksLibrary.Application.Commands.CreateUser;
using TasksLibrary.Application.Commands.GenerateToken;
using TasksLibrary.Application.Commands.Login;
using TasksLibrary.Application.Commands.VerifyToken;
using TasksLibrary.Architecture.Application;
using TasksLibrary.Models;
using TasksLibrary.Models.Interfaces;
using TasksLibrary.Services;

namespace AdeNote.Controllers
{
    /// <summary>
    /// This handles the authentication.
    /// 
    /// 
    /// Supports version 1.0
    /// </summary>
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/v{version:apiVersion}/authentication")]
    [ApiVersion("1.0")]
    [ApiController]
    public class AuthenticationController : BaseController
    {
        private IAuthService _authService;
        private IAuthToken _authToken;
        /// <summary>
        /// This is the constructor
        /// </summary>
        /// <param name="container">A container that contains all the built dependencies</param>
        /// <param name="application">An interface used to interact with the layers</param>
        /// <param name="authService">An authentication service </param>
        /// <param name="userIdentity">An interface that interacts with the user. This fetches the current user details</param>
        public AuthenticationController(IContainer container, ITaskApplication application, IUserIdentity userIdentity,
            IAuthService authService) : base(container, application,userIdentity)
        {
            _authService = authService;
            _authToken = container.Resolve<IAuthToken>();
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
        /// <param name="command">An object to sign up new user</param>
        /// <response code ="200"> Returns if user was created</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>A name and email</returns>
        [AllowAnonymous] 
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<CreateUserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp(CreateUserCommand command)
        {
            var response = await Application.ExecuteCommand<CreateUserCommand, CreateUserDTO>(Container, command);
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
        /// <param name="command"></param>
        /// <response code ="200"> Returns if logged in successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>A name and email</returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionTokenResult<LoginDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            var loginResponse = await Application.ExecuteCommand<LoginCommand, LoginDTO>(Container, command);

            var resultResponse = await _authService.IsAuthenticatorEnabled(command.Email);

            var userDetails = await Application.ExecuteCommand<VerifyTokenCommand, TasksLibrary.Services.UserDTO>(Container,new VerifyTokenCommand()
            {
                AccessToken = loginResponse.Data.AccessToken
            });

            if(resultResponse.IsSuccessful && loginResponse.IsSuccessful)
            {
                var tokenResponse = _authService.GenerateMFAToken(userDetails.Data.UserId,command.Email,loginResponse.Data.RefreshToken);
                AddToCookie("Multi-FactorToken", tokenResponse.Data, 2);
                return Ok("Proceed to enter otp from authenticator");
            }

            AddToCookie("AdeNote-RefreshToken", loginResponse.Data.RefreshToken, 6000);

            return loginResponse.Response();
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
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("token")]
        public async Task<IActionResult> GetAccessToken()
        {
            var refreshToken = Request.Cookies["AdeNote-RefreshToken"];

            var revokedResponse = await _authService.IsTokenRevoked(refreshToken);
            if (revokedResponse.NotSuccessful)
                return revokedResponse.Response();

            var command = new GenerateTokenCommand() 
            { 
                RefreshToken = refreshToken
            };

            var response = await Application.ExecuteCommand<GenerateTokenCommand, string>(Container, command);
            return response.Response();
        }

        /// <summary>
        /// Set up two factor authentication using authenticator app or sms
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /authentication/two-factor-authentication
        ///             
        /// </remarks>
        /// <returns>Authenticator key</returns>
        [Produces("application/json")]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<AuthenticatorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("two-factor-authentication")]
        public async Task<IActionResult> SetUpGoogleAuthenticator()
        {
            var resultResponse = await _authService.IsAuthenticatorEnabled(CurrentUser);

            if (resultResponse.IsSuccessful)
                return TasksLibrary.Utilities.ActionResult.Failed("User has set up two factor authentication").Response();

            var response = await _authService.SetAuthenticator(CurrentUser, CurrentEmail);

            return response.Response();
        }

        /// <summary>
        /// Verify otp 
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///                 POST /authentication/two-factor-authentication/verify-key
        /// </remarks>
        /// <param name="totp">Timed based one time password</param>
        /// <returns>Access token</returns>
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [AllowAnonymous]
        [HttpPost("two-factor-authentication/verify-key")]
        public async Task<IActionResult> VerifyTOTP(string totp)
        {
            var token = Request.Cookies["Multi-FactorToken"];

            var detailsResponse = _authService.ReadDetailsFromToken(token);
            if(detailsResponse.NotSuccessful)
                return detailsResponse.Response();

            var response = _authService.VerifyAuthenticatorOTP(detailsResponse.Data.Email, totp);
            if(response.NotSuccessful)
                return response.Response();

            var accessToken = _authToken.GenerateAccessToken(detailsResponse.Data.UserId,detailsResponse.Data.Email);

            AddToCookie("AdeNote-RefreshToken", detailsResponse.Data.RefreshToken, 600000);

            return Ok(accessToken);
        }


        /// <summary>
        /// Get existing qr code
        /// </summary>
        /// <remarks>
        /// Sample request
        ///     
        ///             GET /authentication/two-factor-authentication/key
        /// </remarks>
        /// <returns>qr ucode url</returns>
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpGet("two-factor-authentication/key")]
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
        /// <returns>Action result</returns>
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TasksLibrary.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("sign-out")]
        public async Task<IActionResult> LogOut()
        {
            var refreshToken = Request.Cookies["AdeNote-RefreshToken"];

            var response = await _authService.RevokeRefreshToken(CurrentUser, refreshToken);

            Response.Cookies.Delete("AdeNote-RefreshToken");

            return response.Response();
        }


        [NonAction]
        private void AddToCookie(string name,string data, double time)
        {
            Response.Cookies.Append(name, data,
                  new CookieOptions()
                  {
                      Expires = DateTime.UtcNow.AddMinutes(time),
                      Secure = true,
                      SameSite = SameSiteMode.None
                  });
        }
    }
}
