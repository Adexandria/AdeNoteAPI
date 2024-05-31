using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class TicketCreateDto
    {
        [Required(ErrorMessage = "Enter issue")]
        public string Issue { get; set; }

        public string Description { get; set; }

        public IFormFile Image { get; set; }
    }
}
