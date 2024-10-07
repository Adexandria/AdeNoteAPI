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
           return configuration.GetSection("AzureAd").Get<AzureAdConfiguration>() ?? throw new NullReferenceException(nameof(AzureAdConfiguration));
        }

        private static TranslateConfiguration ExtractTranslateConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection("TextTranslationConfiguration").Get<TranslateConfiguration>() ?? throw new NullReferenceException(nameof(TranslateConfiguration));
        }

        private static HangFireUserConfiguration ExtractHangFireUserConfiguration(IConfiguration configuration)
        {
           return configuration.GetSection("HangfireUser").Get<HangFireUserConfiguration>() ?? throw new NullReferenceException(nameof(HangFireUserConfiguration));
        }

        private static DefaultConfiguration ExtractDefaultConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection("DefaultConfiguration").Get<DefaultConfiguration>() ?? throw new NullReferenceException(nameof(DefaultConfiguration));
        }

        private static Messaging ExtractMessagingConfiguration(IConfiguration configuration)
        {
            return configuration.GetSection("RabbitMq").Get<Messaging>() ?? throw new NullReferenceException(nameof(Messaging));
        }

        private static string ExtractConnectionString(IConfiguration configuration)
        {
            return configuration.GetConnectionString("NotesDB") ?? throw new NullReferenceException("Invalid Connection string");
        }

        private static string ExtractTokenSecret(IConfiguration configuration)
        {
            return configuration["TokenSecret"] ?? throw new NullReferenceException("Invalid token secret");
        }

        private static Cache ExtractCacheConfiguration(IConfiguration configuration)
        {
            var hostName = configuration["CacheHostName"] ?? throw new NullReferenceException("Invalid host name");

            return new Cache(hostName) ??throw new NullReferenceException(nameof(Cache));
        }

        private static CachingKeys ExtractCachingKeys(IConfiguration configuration)
        {
            return  configuration.GetSection("CachingKeys").Get<CachingKeys>() ?? throw new NullReferenceException("Invalid host name");
        }

        public static Cdn ExtractCdnConfiguration(IConfiguration configuration)
        {
            var endpoint = configuration["CdnEndpoint"] ?? throw new NullReferenceException("Invalid host name");

            return new Cdn(endpoint) ?? throw new NullReferenceException(nameof(endpoint));
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
                HangFireUserConfiguration = ExtractHangFireUserConfiguration(configuration),
                CachingKeys = ExtractCachingKeys(configuration),
                CdnEndpoint = ExtractCdnConfiguration(configuration)
            };
        }
    }
}
