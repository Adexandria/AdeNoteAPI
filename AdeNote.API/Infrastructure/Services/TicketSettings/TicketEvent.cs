using System.Text.Json.Serialization;

namespace AdeNote.Infrastructure.Services.TicketSettings
{
    public class TicketEvent
    {
        [JsonPropertyName("emailAddress")]
        public string EmailAddress {  get; set; }

        [JsonPropertyName("issue")]
        public string Issue { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }

        [JsonPropertyName("lastName")]
        public string LastName { get; set; }

        [JsonPropertyName("ticketId")]
        public string TicketId { get; set; }

        [JsonPropertyName("dateSubmitted")]
        public string DateSubmitted { get; set; }

        [JsonPropertyName("lastUpdated")]
        public string DateUpdated { get; set; }

        [JsonPropertyName("agent")]
        public string Admin { get; set; }
    }
}
