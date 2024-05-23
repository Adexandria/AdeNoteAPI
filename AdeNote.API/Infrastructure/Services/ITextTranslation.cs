using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using AdeText.Models;

namespace AdeNote.Infrastructure.Services
{
    public interface ITextTranslation
    {
        public Task<ActionResult<string[]>> TranslatePage(string pageContent, string translatedLanguage);

        public Task<ActionResult<string>> TransliteratePage(string pageContent, string translatedLanguage, string transliteratedLanguage);

        public ActionResult<ILanguage> GetSupportedLanguages(string firstScope, string secondScope, string etag = null);
    }
}
