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
        public abstract void Publish(string message, string topic);
        public abstract void Subscribe(string topic);
    }
}
