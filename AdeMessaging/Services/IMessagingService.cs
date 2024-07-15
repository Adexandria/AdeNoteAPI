using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeMessaging.Services
{
    public interface IMessagingService
    {
        void Publish(string message, string topic);

        void Subscribe(string topic);

        bool IsConnected();
    }
}
