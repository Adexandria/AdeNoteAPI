using Excelify.Services.Utility.Attributes;

namespace AdeNote.Models.DTOs
{
    /// <summary>
    /// DTO to display the book
    /// </summary>
    public class BookDTO
    {

        /// <summary>
        /// Id of the book
        /// </summary>
        [Excelify("id")]
        public Guid Id { get; set; }


        /// <summary>
        /// Title of the book
        /// </summary>
        [Excelify("title")]
        public string Title { get; set; }


        /// <summary>
        /// Description of the book
        /// </summary>
        [Excelify("description")]
        public string Description { get; set; }

        /// <summary>
        /// A list of pages
        /// </summary>
        public IList<BookPages> BookPages { get; set; }
    }
}
