namespace AdeNote.Models.DTOs
{
    public class TicketsDTO
    {
        public Guid TicketId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Issue { get; set; }

        public string Status { get; set; }

        public string Created { get; set; }
    }
}
