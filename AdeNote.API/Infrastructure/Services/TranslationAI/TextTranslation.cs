using AdeNote.Infrastructure.Utilities;
using AdeText.Models;
using AdeText.Services;

namespace AdeNote.Infrastructure.Services.TranslationAI
{
    public class TextTranslation : ITextTranslation
    {
        public TextTranslation(ITranslateClient translateClient)
        {
            _translateClient = translateClient;
        }
        public async Task<ActionResult<string[]>> TranslatePage(string pageContent, string translatedLanguage, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(pageContent))
            {
                return ActionResult<string[]>.Failed("Invalid page content");
            }

            var detectedlanguage = await _translateClient.DetectLanguage(pageContent);

            if (detectedlanguage == null)
            {
                return ActionResult<string[]>.Failed($"Failed to translate to {translatedLanguage}, Service Unavailable Try again later", StatusCodes.Status400BadRequest);
            }

            if (detectedlanguage.Language == translatedLanguage)
            {
                return ActionResult<string[]>.Failed($"Failed to translate to {translatedLanguage}, Text is already in this language", StatusCodes.Status400BadRequest);
            }

            var translatedText = await _translateClient.TranslateLanguage(pageContent, translatedLanguage, detectedlanguage.Language, cancellationToken);

            if (!translatedText.Translations.Any())
            {
                return ActionResult<string[]>.Failed($"Failed to translate to {translatedLanguage}, Service Unavailable Try again later", StatusCodes.Status400BadRequest);
            }

            return ActionResult<string[]>.SuccessfulOperation
                (new[] { translatedText.Translations.FirstOrDefault().Text, detectedlanguage.Language });

        }

        public ActionResult<ILanguage> GetSupportedLanguages(string firstScope, string secondScope, string _etag, CancellationToken cancellationToken = default)
        {
            var supportedLanguages = _translateClient.GetSupportedLanguages(new[] { firstScope, secondScope }, _etag, cancellationToken);

            if (supportedLanguages == null)
            {
                return ActionResult<ILanguage>.Failed("Failed to get supported languages");
            }

            return ActionResult<ILanguage>.SuccessfulOperation(supportedLanguages);
        }

        public async Task<ActionResult<string>> TransliteratePage(string pageContent, string translatedLanguage, string transliteratedLanguage, CancellationToken cancellationToken = default)
        {
            var transliteratedPage = await _translateClient.TransliterateLanguage(pageContent, translatedLanguage, transliteratedLanguage, cancellationToken);

            if (transliteratedPage == null)
            {
                return ActionResult<string>.Failed("Failed to get supported languages");
            }

            return ActionResult<string>.SuccessfulOperation(transliteratedPage.Text);
        }

        private readonly ITranslateClient _translateClient;
    }
}
