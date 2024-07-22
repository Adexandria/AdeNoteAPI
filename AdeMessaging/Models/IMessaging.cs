using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeMessaging.Models
{
    public interface IMessaging
    {
        ExchangeType ExchangeType { get; set; }
        string HostName { get; set; }
        string VirtualHost { get; set; }
        string UserName { get; set; }   
        string Password { get; set; }
        int Port { get; set; }
    }
}
