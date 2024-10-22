using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class CreateThreadDto
    {
        [Required]
        public string Message { get; set; }
    }
}
