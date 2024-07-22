using AdeMessaging.Services;
using AdeNote.Infrastructure.Utilities.EventSystem;

namespace AdeNote.Infrastructure.Extension
{
    public static class EventExtension
    {

        public static void SetUpRabbitConfiguration(this WebApplication app, IConfiguration config)
        {
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;

            var messagingEvent = serviceProvider.GetRequiredService<IMessagingService>();

            var eventConfiguration = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>();


            messagingEvent.CreateExchange(eventConfiguration.Exchange);

            messagingEvent.CreateQueue(eventConfiguration.Queue, eventConfiguration.Exchange, eventConfiguration.RoutingKey);

            
        }
    }
}
