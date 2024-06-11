using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to update a label
    /// </summary>
    public class LabelUpdateDTO 
    {
        [Required(ErrorMessage ="Enter label")]
        /// <summary>
        /// Title of a label
        /// </summary>
        public string Title { get; set; }
    }
}
