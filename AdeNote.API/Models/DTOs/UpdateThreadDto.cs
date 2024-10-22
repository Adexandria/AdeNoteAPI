using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class UpdateThreadDto
    {
        [Required]
        public string Message { get; set; }
    }
}
