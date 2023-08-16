namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to create a note
    /// </summary>
    public class CreateNoteDTO
    {
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
