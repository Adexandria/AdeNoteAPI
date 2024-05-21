using AdeText.Models;

namespace AdeText.Services
{
    public interface ITranslateClient
    {
        public Task<IDetectLanguage> DetectLanguage(string text);
        public Task<ITranslateLanguage> TranslateLanguage(string text, string to, string from = null);
        public Task<ITranslateLanguage> TranslateLanguage(string text, string[] to, string from = null);
        public ILanguage GetSupportedLanguages(string scope = "translation", string _etag = null);
    }
}
