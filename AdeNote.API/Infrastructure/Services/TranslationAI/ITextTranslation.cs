using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;
using AdeText.Models;

namespace AdeNote.Infrastructure.Services.TranslationAI
{
    public interface ITextTranslation
    {
        public Task<ActionResult<string[]>> TranslatePage(string pageContent, string translatedLanguage, CancellationToken cancellationToken = default);

        public Task<ActionResult<string>> TransliteratePage(string pageContent, string translatedLanguage, string transliteratedLanguage, CancellationToken cancellationToken = default);

        public ActionResult<ILanguage> GetSupportedLanguages(string firstScope, string secondScope, string etag = null, CancellationToken cancellationToken = default);
    }
}
