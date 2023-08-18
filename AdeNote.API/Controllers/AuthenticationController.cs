using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Services;
using AdeNote.Infrastructure.Utilities;
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
        private IUserService _userService;
        private IAuthToken _authToken;
        /// <summary>
        /// This is the constructor
        /// </summary>
        /// <param name="container">A container that contains all the built dependencies</param>
        /// <param name="application">An interface used to interact with the layers</param>
        public AuthenticationController(IContainer container, ITaskApplication application, IUserIdentity userIdentity,
            IAuthService authService, IUserService userService) : base(container, application,userIdentity)
        {
            _authService = authService;
            _userService = userService;
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

            var userDetails = await Application.ExecuteCommand<VerifyTokenCommand, UserDTO>(Container,new VerifyTokenCommand()
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


        [HttpPost("two-factor-authentication")]
        public async Task<IActionResult> SetUpGoogleAuthenticator()
        {
            var resultResponse = await _authService.IsAuthenticatorEnabled(CurrentUser);

            if (resultResponse.IsSuccessful)
                return new BadRequestObjectResult("User has set up two factor authentication");

            var currentUserResponse = await _userService.FetchUserById(CurrentUser);

            if (currentUserResponse.NotSuccessful)
                return currentUserResponse.Response();

            var response = await _authService.SetEmailAuthenticator(CurrentUser, currentUserResponse.Data.Email);

            return response.Response();
        }

        [AllowAnonymous]
        [HttpPost("two-factor-authentication/verify-key")]
        public async Task<IActionResult> VerifyTOTP(string totp)
        {
            var token = Request.Cookies["Multi-FactorToken"];

            var detailsResponse = _authService.ReadDetailsFromToken(token);
            if(detailsResponse.NotSuccessful)
                return detailsResponse.Response();

            var response = _authService.VerifyEmailAuthenticator(detailsResponse.Data.Email, totp);
            if(response.NotSuccessful)
                return response.Response();

            var accessToken = _authToken.GenerateAccessToken(detailsResponse.Data.UserId,detailsResponse.Data.Email);

            AddToCookie("AdeNote-RefreshToken", detailsResponse.Data.RefreshToken, 600000);

            return Ok(accessToken);
        }


        [HttpGet("two-factor-authentication/key")]
        public async Task<IActionResult> GetAuthenticatorQRCode()
        {
            var response = await _authService.GetUserQrCode(CurrentUser);
            return response.Response();
        }


        [HttpPost("sign-out")]
        public async Task<IActionResult> SignOut()
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
