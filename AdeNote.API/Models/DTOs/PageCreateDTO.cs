using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// A DTO to create a page
    /// </summary>
    public class PageCreateDTO
    {

        /// <summary>
        /// A title of a page
        /// </summary>
        [Required]
        public string Title { get; set; }
        /// <summary>
        /// A content of a page
        /// </summary>
        [Required]
        public string Content { get; set; }
    }
}
