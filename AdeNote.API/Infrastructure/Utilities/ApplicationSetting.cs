using AdeNote.Infrastructure.Utilities.AI;
using AdeNote.Infrastructure.Utilities.AuthenticationFilter;
using AdeNote.Infrastructure.Utilities.CacheModel;
using AdeNote.Infrastructure.Utilities.EventSystem;
using AdeNote.Infrastructure.Utilities.SSO;

namespace AdeNote.Infrastructure.Utilities
{
    public class ApplicationSetting
    {
        public Messaging Messaging { get; set; }
        public TranslateConfiguration TranslateConfiguration { get; set; }
        public Cache CacheConfiguration { get; set; }
        public string TokenSecret { get; set; }
        public AzureAdConfiguration AzureAdConfiguration { get; set; }
        public string ConnectionString { get; set; }
        public DefaultConfiguration DefaultConfiguration { get; set; }
        public UserConfiguration HangFireUserConfiguration { get; set; }
        public CachingKeys CachingKeys { get; set; }
    }
}
