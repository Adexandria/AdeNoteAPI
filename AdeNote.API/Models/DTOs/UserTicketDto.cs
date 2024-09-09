namespace AdeNote.Models.DTOs
{
    public class UserTicketDto
    {
        public string TicketId {  get; set; }

        public string Issue { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public string Status { get; set; }

        public string Created { get; set; }

        public string Modified {  get; set; }

        public string Handler { get; set; }
    }
}
