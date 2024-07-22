using AdeNote.Infrastructure.Utilities;
using AdeNote.Infrastructure.Utilities.AI;
using AdeNote.Infrastructure.Utilities.AuthenticationFilter;
using AdeNote.Infrastructure.Utilities.CacheModel;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Infrastructure.Utilities.SSO;
using Microsoft.Extensions.Caching.Memory;

namespace AdeNote.Infrastructure.Extension
{
    public static class ConfigurationExtension
    {
        private static AzureAdConfiguration ExtractAzureAdConfiguration(IConfiguration configuration)
        {
           return configuration.GetSection("AzureAd").Get<AzureAdConfiguration>();
        }

        private static TranslateConfiguration ExtractTranslateConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection("TextTranslationConfiguration").Get<TranslateConfiguration>();
        }

        private static HangFireUserConfiguration ExtractHangFireUserConfiguration(IConfiguration configuration)
        {
           return configuration.GetSection("HangfireUser").Get<HangFireUserConfiguration>();
        }

        private static DefaultConfiguration ExtractDefaultConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection("DefaultConfiguration").Get<DefaultConfiguration>();
        }

        private static Messaging ExtractMessagingConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection("RabbitMq").Get<Messaging>();
        }

        private static string ExtractConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("NotesDB");
        }

        private static string ExtractTokenSecret(IConfiguration configuration)
        {
            return configuration["TokenSecret"];
        }

        private static Cache ExtractCacheConfiguration(IConfiguration configuration)
        {
            var hostName = configuration["CacheHostName"];

            return new Cache(hostName);
        }


        public static ApplicationSetting ExtractApplicationSetting(this IConfiguration configuration)
        {
            return new ApplicationSetting
            {
                AzureAdConfiguration = ExtractAzureAdConfiguration(configuration),
                TranslateConfiguration = ExtractTranslateConfiguration(configuration),
                CacheConfiguration = ExtractCacheConfiguration(configuration),
                Messaging = ExtractMessagingConfiguration(configuration),
                TokenSecret = ExtractTokenSecret(configuration),
                ConnectionString = ExtractConnectionString(configuration),
                DefaultConfiguration = ExtractDefaultConfiguration(configuration),
                HangFireUserConfiguration = ExtractHangFireUserConfiguration(configuration)
            };
        }
    }
}
