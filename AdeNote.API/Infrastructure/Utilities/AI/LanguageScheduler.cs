using AdeCache.Services;
using AdeNote.Infrastructure.Services.TranslationAI;
using AdeText.Models;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace AdeNote.Infrastructure.Utilities.AI
{
    public class LanguageScheduler
    {
        public LanguageScheduler(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider.CreateScope().ServiceProvider;
        }
        public void GetLanguages()
        {
            RecurringJob.AddOrUpdate("GetLanguages",() => GetSupportedLanaguages(), "0 12 * * *");
        }

       public async Task GetSupportedLanaguages()
       {
            await Task.Run(() =>
            {

                var textTranslation = _serviceProvider.GetRequiredService<ITextTranslation>();

                var cacheService = _serviceProvider.GetRequiredService<ICacheService>();

                var loggerFactory = _serviceProvider.GetRequiredService<ILoggerFactory>();

                var logger = loggerFactory.CreateLogger<LanguageScheduler>();

                var etag = cacheService.Get<string>("etag")?.ToString();

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
                    cacheService.Set("etag", response.Data.ETag);
                    cacheService.Set("translation_languages", response.Data.TranslationLanguages);
                    cacheService.Set("transliteration_languages", response.Data.TransliterationLanguages);
                    return;
                }

                logger.LogError(new Exception(response.Errors[0]),"Failed to update languages");
            });
        }
        private readonly IServiceProvider _serviceProvider;
    }
}
