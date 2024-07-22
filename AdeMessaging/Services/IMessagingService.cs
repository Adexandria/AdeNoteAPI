using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeMessaging.Services
{
    public interface IMessagingService
    {
        string Publish(string message, string exchange, string routingKey);

        string Subscribe(string exchange, string queue, string routingKey);

        void CreateQueue(string queue, string exchange, string routingKey);
        void CreateExchange(string exchange);

        bool IsConnected();
    }
}
