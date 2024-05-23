﻿using AdeText.Models;

namespace AdeText.Services
{
    public interface ITranslateClient
    {
        public Task<IDetectLanguage> DetectLanguage(string text);
        public Task<ITranslateLanguage> TranslateLanguage(string text, string to, string from = null);
        public Task<ITranslateLanguage> TranslateLanguage(string text, string[] to, string from = null);
        public Task<ITranslateLanguage> TransliterateLanguage(string text, string toLanguage, string fromScript);
        public ILanguage GetSupportedLanguages(string _etag = null, params string[] scope);
    }
}
