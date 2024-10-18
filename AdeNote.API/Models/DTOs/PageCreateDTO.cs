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
        [Required(ErrorMessage = "Enter page name")]
        public string Title { get; set; }
        /// <summary>
        /// A content of a page
        /// </summary>
        [Required(ErrorMessage = "Enter content")]
        public string Content { get; set; }

        public IFormFile File { get; set; }

        public string Description { get; set; }
    }
}
