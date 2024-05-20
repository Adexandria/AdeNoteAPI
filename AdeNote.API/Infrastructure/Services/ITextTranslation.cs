using AdeNote.Infrastructure.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface ITextTranslation
    {
        public Task<ActionResult<string>> TranslatePage(string pageContent, string translatedLanguage);
    }
}
