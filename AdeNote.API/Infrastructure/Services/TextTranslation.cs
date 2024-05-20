using AdeNote.Infrastructure.Utilities;
using AdeText.Services;

namespace AdeNote.Infrastructure.Services
{
    public class TextTranslation : ITextTranslation
    {
        public TextTranslation(ITranslate translateClient)
        {
            _translateClient = translateClient;
        }
        public async Task<ActionResult<string>> TranslatePage(string pageContent, string translatedLanguage)
        {
            try
            {
                if (string.IsNullOrEmpty(pageContent))
                {
                    return ActionResult<string>.Failed("Invalid page content");
                }

                var detectedlanguage = await _translateClient.DetectLanguage(pageContent);

                if (detectedlanguage == null)
                {
                    return ActionResult<string>.Failed($"Failed to translate to {translatedLanguage}");
                }

                var translatedText = await _translateClient.TranslateLanguage(pageContent, translatedLanguage, detectedlanguage.Language);

                if(!translatedText.Translations.Any())
                {
                    return ActionResult<string>.Failed($"Failed to translate to {translatedLanguage}");
                }

                return ActionResult<string>.SuccessfulOperation(translatedText.Translations.FirstOrDefault().Text);

            }
            catch (Exception ex)
            {
                return ActionResult<string>.Failed(ex.Message);
            }
        }

        private readonly ITranslate _translateClient;
    }
}
