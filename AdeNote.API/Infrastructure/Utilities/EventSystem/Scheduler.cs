using AdeMessaging.Services;
using AdeMessaging.Services.Extensions;
using AdeNote.Infrastructure.Services.Notification;
using AdeNote.Infrastructure.Services.TicketSettings;
using AdeNote.Infrastructure.Utilities.EmailSettings;
using AdeNote.Models;
using Hangfire;
using System.Text.Json;

namespace AdeNote.Infrastructure.Utilities.EventSystem
{
    public class Scheduler
    {
        public Scheduler(IServiceProvider _serviceProvider)
        {
            serviceProvider = _serviceProvider.CreateScope().ServiceProvider;
        }

        public void Subscribe(EventConfiguration eventConfiguration)
        {
            RecurringJob.AddOrUpdate("GetMessage", () => GetMessage(eventConfiguration), "*/5 * * * *");;
        }
        public void GetMessage(EventConfiguration eventConfiguration)
        {
            var messagingEvent = serviceProvider.GetRequiredService<IMessagingService>();

            var notificationService = serviceProvider.GetRequiredService<INotificationService>();

            var message = messagingEvent.Subscribe(eventConfiguration.Exchange, eventConfiguration.Queue ,eventConfiguration.RoutingKey);

            if (string.IsNullOrEmpty(message))
            {
                return;
            }
            var ticket = JsonSerializer.Deserialize<TicketEvent>(message);

            var template = EmailTemplate.TicketUpdateNotification;

            var substitution = new Dictionary<string, string>()
            {
                { "{{FirstName}}", ticket.FirstName},
                { "{{LastName}}",ticket.LastName },
                { "{{TicketId}}", ticket.TicketId },
                { "{{Issue}}", ticket.Issue},
                { "{{Current_Status}}", ticket.Status },
                {"{{Assigned_Agent}}" , ticket.Admin},
                {"{{Last_Update_Date}}", ticket.DateUpdated },
                {"{{Ticket_Submission_Date}}",ticket.DateSubmitted },
                {"{{SupportEmail}}", "adeolaaderibigbe09@gmail.com"},
                { "{{Support_Email}}", "AdeNote Support team" },
                {"{{Your_Company}}" , "AdeNote"},
                {"{{Contact_Information}}", "adeolaaderibigbe09@gmail.com" },
                {"{{Current_Year}}",DateTime.Now.Year.ToString() }
            };

            if(ticket.Status == Status.Resolved.GetDescription())
            {
                template = EmailTemplate.SolvedTicketNotification;
                substitution.Add("{{ResolutionDetails}}", ticket.ResolutionDetails ?? "None");
            }

            var email = new Email(ticket.EmailAddress, "Ticket Status Update");

            notificationService.SendNotification(email, template, ContentType.html, substitution);
        }


        private readonly IServiceProvider serviceProvider;
    }
}
