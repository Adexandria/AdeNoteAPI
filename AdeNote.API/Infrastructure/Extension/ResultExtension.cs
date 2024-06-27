using AdeNote.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace AdeNote.Infrastructure.Extension
{
    /// <summary>
    /// This extension for the custom action result
    /// </summary>
    public static class ResultExtension
    {
        /// <summary>
        /// This maps the custom action result to mvc action result
        /// </summary>
        /// <param name="actionResult">A custom action result object</param>
        /// <returns>An interface of MVC Action result</returns>
        public static IActionResult Response(this Utilities.ActionResult actionResult)
        {
            var result = actionResult.StatusCode switch
            {
                (int)HttpStatusCode.OK => new OkObjectResult(actionResult),
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(actionResult.Errors),
                (int)HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(actionResult.Errors),
                (int)HttpStatusCode.NotFound => new NotFoundObjectResult(actionResult.Errors),
                _ => new ObjectResult(actionResult.Errors)
            };

            if (actionResult.StatusCode == (int)HttpStatusCode.InternalServerError)
                result.StatusCode = 500;
         

            return result;
        }

        public static IActionResult Response<T>(this Utilities.ActionResult<T> actionResult)
        {
            var result = actionResult.StatusCode switch
            {
                (int)HttpStatusCode.OK => new OkObjectResult(actionResult.Data),
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(actionResult.Errors),
                (int)HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(actionResult.Errors),
                (int)HttpStatusCode.NotFound => new NotFoundObjectResult(actionResult.Errors),
                _ => new ObjectResult(actionResult.Errors)
            };

            if (actionResult.StatusCode == (int)HttpStatusCode.InternalServerError)
                result.StatusCode = 500;


            return result;
        }
    }
}
