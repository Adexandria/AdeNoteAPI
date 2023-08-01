using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// A DTO that is used to create a new book
    /// </summary>
    public class BookCreateDTO
    {
        /// <summary>
        /// Title of the book
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Description of the book
        /// </summary>
        [Required]
        public string Description { get; set; }
    }
}
