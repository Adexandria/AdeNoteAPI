using AdeText.Services;

namespace AdeNote.Infrastructure.Utilities.AI
{
    public class TranslateConfiguration : ITranslateConfiguration
    {
        public TranslateConfiguration()
        {
                
        }
        public TranslateConfiguration(string key, string location, string endpoint, int retryConfiguration)
        {
            Key = key; 
            Location = location; 
            Endpoint = endpoint; 
            RetryConfiguration = retryConfiguration;
        }
        public string Key { get; set; }
        public string Location { get; set; }
        public string Endpoint { get; set; }
        public int RetryConfiguration { get; set; }
    }
}
