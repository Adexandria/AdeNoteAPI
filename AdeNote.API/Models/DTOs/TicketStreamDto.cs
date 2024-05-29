using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class TicketStreamDto
    {
        [Required(ErrorMessage = "Enter issue")]
        public string Issue { get; set; }

        public string Description { get; set; }

        public Stream Image  { get; set; }
    }
}
