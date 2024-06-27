using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// A DTO to create a label
    /// </summary>
    public class LabelCreateDTO
    {
        /// <summary>
        /// Title of a label
        /// </summary>
        [Required(ErrorMessage = "Enter label")]
        public string Title { get; set; }
    }
}
