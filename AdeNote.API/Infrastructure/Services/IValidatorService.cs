using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IValidatorService
    {
        // Use transient di 
        // but how will you pass in values??
        // using static methods??
        //
        ActionResult<string> Validate();
    }
}
