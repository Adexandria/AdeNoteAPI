namespace AdeNote.Models.DTOs
{
    public class TicketDTO
    {
        public Guid TicketId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Issue {  get; set; }

        public Guid Issuer { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Status { get; set; }
    }
}
