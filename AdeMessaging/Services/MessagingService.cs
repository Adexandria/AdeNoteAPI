using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeMessaging.Services
{
    public abstract class MessagingService : IMessagingService
    {
        public abstract bool IsConnected();
        public abstract void CreateExchange(string exchange);
        public abstract void CreateQueue(string queue, string exchange, string routingKey);
        public abstract string Publish(string message, string exchange, string routingKey);
        public abstract string Subscribe(string exchange, string queue, string routingKey);
    }
}
