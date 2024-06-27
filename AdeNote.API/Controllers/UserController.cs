using AdeNote.Infrastructure.Extension;
using AdeNote.Models.DTOs;
using AdeNote.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using System.ComponentModel.DataAnnotations;
using AdeNote.Infrastructure.Utilities.UserConfiguation;
using AdeNote.Infrastructure.Services.Authentication;

namespace AdeNote.Controllers
{
    [Route("api/v{version:apiVersion}/user")]
    [ApiVersion("1.0")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IAuthService _authService;
        public UserController(IUserIdentity userIdentity,
            IAuthService authService) : base(userIdentity)
        {
            _authService = authService;
        }


        /// <summary>
        /// Generate new recovery code
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///             POST /user/recovery-codes
        /// </remarks>
        /// <response code ="200"> Returns if password was changed successfully</response>
        /// <response code ="400"> Returns if experiencing client issues</response> 
        /// <response code ="404"> Returns if user can't be found</response> 
        /// <returns>Action result</returns>
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string[]>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string[]>), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string[]>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<string[]>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("recovery-codes")]
        public async Task<IActionResult> GenerateRecoveryCode()
        {
            var response = await _authService.GenerateRecoveryCodes(CurrentUser);
            return response.Response();
        }


        /// <summary>
        /// Sets up two factor authentication using google authenticator app
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /user/two-factor-authentication/google
        ///             
        /// </remarks>
        /// <response code ="200"> Returns if two factor authenticator was successfully set up</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        /// <returns>Authenticator key</returns>
        [Produces("application/json")]
        [Authorize("User")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult<AuthenticatorDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("two-factor-authentication/google")]
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
        ///             POST /user/two-factor-authentication/sms
        ///             
        /// </remarks>
        /// <response code ="200"> Returns if two factor authenticator was successfully set up</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Authorize("User")]
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
        ///             POST /user/phonenumber
        ///             
        /// </remarks>
        /// <param name="phoneNumber">User phone number</param>
        /// <response code ="200"> Returns if phone number was added</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Authorize("User")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("phonenumber")]
        public async Task<IActionResult> AddPhoneNumber([Required(ErrorMessage = "Invalid phone number")]string phoneNumber)
        {
            var response = await _authService.SetPhoneNumber(CurrentUser, phoneNumber);

            return response.Response();
        }

        /// <summary>
        /// Verifies phone number
        /// </summary>
        /// <remarks>
        /// Sample request: 
        /// 
        ///             POST /user/verify-phonenumber
        ///             
        /// </remarks>
        /// <param name="verificationCode">Verification code</param>
        /// <response code ="200"> Returns if phone number was verified</response>
        /// <response code ="400"> Returns if experiencing client issues</response>
        /// <response code ="500"> Returns if experiencing server issues</response>
        /// <response code ="404"> Returns if parameters not found</response>
        /// <response code ="401"> Returns if unauthorised</response>
        [Produces("application/json")]
        [Authorize("User")]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(Infrastructure.Utilities.ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        [HttpPost("verify-phonenumber")]
        public async Task<IActionResult> VerifyPhoneNumber([Required(ErrorMessage ="Invalid verification code")]string verificationCode)
        {
            var resultResponse = await _authService.IsPhoneNumberVerified(CurrentUser);

            if (resultResponse.IsSuccessful)
                return Infrastructure.Utilities
                    .ActionResult.Failed("Phone number is already verified", StatusCodes.Status400BadRequest)
                    .Response();

            var response = await _authService.VerifyPhoneNumber(CurrentUser, verificationCode);

            return response.Response();
        }
    }
}
