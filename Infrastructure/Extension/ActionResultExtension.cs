using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AdeNote.Infrastructure.Extension
{
    public static class ActionResultExtension
    {
        public static IActionResult Response(this TasksLibrary.Utilities.ActionResult actionResult)
        {
            var result = actionResult.StatusCode switch
            {
                (int)HttpStatusCode.OK => new OkObjectResult(actionResult),
                (int)HttpStatusCode.BadRequest => new BadRequestObjectResult(actionResult),
                (int)HttpStatusCode.Unauthorized => new UnauthorizedObjectResult(actionResult),
                (int)HttpStatusCode.InternalServerError => new ObjectResult(actionResult),
                (int)HttpStatusCode.NotFound => new NotFoundObjectResult(actionResult),
            };

            if (actionResult.StatusCode == (int)HttpStatusCode.InternalServerError)
                result.StatusCode = 500;

            return result;
        }
    }
}
