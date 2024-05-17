using AdeNote.Infrastructure.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdeNote.Infrastructure.Extension
{
    /// <summary>
    /// This extension for the custom action result
    /// </summary>
    public static class ActionResultExtension
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
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(actionResult),
                (int)HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(actionResult),
                (int)HttpStatusCode.NotFound => new NotFoundObjectResult(actionResult),
                _ => new ObjectResult(actionResult),
            };

            if (actionResult.StatusCode == (int)HttpStatusCode.InternalServerError)
                result.StatusCode = 500;

            return result;
        }
    }
}
