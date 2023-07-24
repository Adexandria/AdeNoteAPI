using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    public class BookCreateDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
