namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to update a book
    /// </summary>
    public class BookUpdateDTO 
    {
        /// <summary>
        /// Title of book
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Description of book
        /// </summary>
        public string Description { get; set; }
    }
}
