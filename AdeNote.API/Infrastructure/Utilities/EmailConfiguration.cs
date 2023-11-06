namespace AdeNote.Infrastructure.Utilities
{
    /// <summary>
    /// Email condiguration for the application
    /// </summary>
    public class EmailConfiguration
    {

        public EmailConfiguration()
        {

        }

        public EmailConfiguration(string domain, string url, string from, string apikey)
        {
           Domain = domain;
           URL = url;
            From = from;
            APIKey = apikey;
        }
        /// <summary>
        /// Domain
        /// </summary>
        public string Domain { get; set; }
        /// <summary>
        /// Url
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// Email address of the sender
        /// </summary>
        public string From { get; set; }

        /// <summary>
        /// API Key
        /// </summary>
        public string APIKey { get; set; }
    }
}
