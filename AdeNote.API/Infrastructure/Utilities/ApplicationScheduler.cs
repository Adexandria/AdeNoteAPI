using AdeCache.Services;
using AdeNote.Infrastructure.Services.TranslationAI;
using AdeNote.Infrastructure.Utilities.AI;
using AdeNote.Infrastructure.Utilities.EventSystem;

namespace AdeNote.Infrastructure.Utilities
{
    public static class ApplicationScheduler
    {
        public static void ScheduleService(this WebApplication app, IConfiguration config)
        {
            var serviceProvider = app.Services;

            var languageScheduler = serviceProvider.GetRequiredService<LanguageScheduler>();
           
            languageScheduler.GetLanguages();

            var scheduler = serviceProvider.GetRequiredService<Scheduler>();

            var ticketConfigurartion = config.GetSection("Events")
                .GetSection("Ticket")
                .Get<EventConfiguration>();

            scheduler.Subscribe(ticketConfigurartion);
        }
    }
}
