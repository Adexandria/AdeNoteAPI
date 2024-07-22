using AdeMessaging.Models;
using AdeMessaging.Services;
using AdeMessaging.Services.Exceptions;
using Microsoft.Extensions.Logging;
using System.Reflection;


namespace AdeMessaging
{
    public static class MessagingFactory
    {
        public static IMessagingService CreateServices(IMessaging messaging, ILoggerFactory loggerFactory ,Assembly currentAssembly = null)
        {
            Type messagingType = currentAssembly == null ? Assembly.GetExecutingAssembly()
                .GetTypes().Where(type => type.BaseType == typeof(MessagingService)
                && !type.IsAbstract).FirstOrDefault()!
                : currentAssembly.GetTypes().Where(type => type.BaseType == typeof(MessagingService)
                && !type.IsAbstract).FirstOrDefault()!;

            if (messagingType == null)
            {
                throw new MessagingException("Failed to create messaging service");
            }

            IMessagingService service = Activator.CreateInstance(messagingType, messaging, loggerFactory) as IMessagingService ?? throw new NullReferenceException("name");

            if (service.IsConnected())
            {
                return service;
            }

            throw new MessagingException("Failed to create messaging service");
        }
    }
}
