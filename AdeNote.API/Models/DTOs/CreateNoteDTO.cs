using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to create a note
    /// </summary>
    public class CreateNoteDTO
    {
        [Required(ErrorMessage = "Enter task")]
        /// <summary>
        /// The note to perform
        /// </summary>
        public string Task { get; set; }

        /// <summary>
        /// The description
        /// </summary>
        public string Description { get; set; }
    }
}
