using AdeText.Models;

namespace AdeText.Services
{
    public interface ITranslateClient
    {
        public Task<IDetectLanguage> DetectLanguage(string text, CancellationToken cancellationToken = default);
        public Task<ITranslateLanguage> TranslateLanguage(string text, string to, string from = null, CancellationToken cancellationToken = default);
        public Task<ITranslateLanguage> TranslateLanguage(string text, string[] to, string from = null, CancellationToken cancellationToken = default);
        public Task<Translation> TransliterateLanguage(string text, string toLanguage, string fromScript, CancellationToken cancellationToken = default);
        public ILanguage GetSupportedLanguages(string[] scopes, string _etag = null, CancellationToken cancellationToken = default);
    }
}
