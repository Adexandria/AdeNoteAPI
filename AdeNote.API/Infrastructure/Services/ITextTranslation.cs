using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using AdeText.Models;

namespace AdeNote.Infrastructure.Services
{
    public interface ITextTranslation
    {
        public Task<ActionResult<string[]>> TranslatePage(string pageContent, string translatedLanguage);

        public ActionResult<ILanguage> GetSupportedLanguages(string scope, string _etag = null);
    }
}
