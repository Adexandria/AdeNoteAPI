namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to display pages in a book
    /// </summary>
    public class BookPages
    {
        /// <summary>
        /// An id of the page
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the book
        /// </summary>
        public string Title { get; set; }
    }
}