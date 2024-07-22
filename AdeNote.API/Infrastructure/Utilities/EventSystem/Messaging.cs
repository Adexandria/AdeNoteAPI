using AdeMessaging.Models;

namespace AdeNote.Infrastructure.Utilities.EventSystem
{
    public class Messaging : IMessaging
    {
        public ExchangeType ExchangeType { get; set; }
        public string HostName { get; set; }
        public string VirtualHost { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
    }
}
