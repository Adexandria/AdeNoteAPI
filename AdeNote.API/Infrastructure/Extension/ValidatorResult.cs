using AdeNote.Infrastructure.Utilities;
using System.ComponentModel.DataAnnotations;

namespace AdeNote.Infrastructure.Extension
{
    public class ValidatorResult : ValidationResult
    {
        public ValidatorResult(ActionResult _actionResult) : base(_actionResult.Errors.SingleOrDefault())
        {
            actionResult = _actionResult;
        }

        public readonly ActionResult actionResult;
    }
}
