﻿using AdeNote.Infrastructure.Services.TranslationAI;
using AdeText.Models;
using Hangfire;
using Microsoft.Extensions.Caching.Memory;

namespace AdeNote.Infrastructure.Utilities.AI
{
    public class Language
    {
        public Language(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider;   
        }
        public void GetLanguages()
        {
            RecurringJob.AddOrUpdate("GetLanguages",() => GetSupportedLanaguages(), "0 12 * * *");
        }

       public async Task GetSupportedLanaguages()
       {
            await Task.Run(() =>
            {
                var textTranslation = serviceProvider.GetRequiredService<ITextTranslation>();

                var memoryCache = serviceProvider.GetRequiredService<IMemoryCache>();

                var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

                var logger = loggerFactory.CreateLogger<Language>();

                var etag = memoryCache.Get("etag")?.ToString();

                ActionResult<ILanguage> response = null;

                if (string.IsNullOrEmpty(etag))
                {
                    response = textTranslation.GetSupportedLanguages("translation", "transliteration");
                }
                else
                {
                    response = textTranslation.GetSupportedLanguages("translation", "transliteration", etag);
                }

                if (response.IsSuccessful)
                {
                    logger.LogInformation("All languages updated successfully");
                    memoryCache.Set("etag", response.Data.ETag);
                    memoryCache.Set("translation_languages", response.Data.TranslationLanguages);
                    memoryCache.Set("transliteration_languages", response.Data.TransliterationLanguages);
                    return;
                }

                logger.LogError(new Exception(response.Errors[0]),"Failed to update languages");

            });
        }
        private IServiceProvider serviceProvider;
    }
}
