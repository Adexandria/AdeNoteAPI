using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdeMessaging.Services.Exceptions
{
    public class MessagingException : Exception
    {
        public MessagingException(string errorMessage) :base(errorMessage)
        {
            
        }
    }
}
