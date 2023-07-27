namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to update a page
    /// </summary>
    public class PageUpdateDTO 
    {
        /// <summary>
        /// A title of a page
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// A content of a page
        /// </summary>
        public string Content { get; set; }
    }
}
