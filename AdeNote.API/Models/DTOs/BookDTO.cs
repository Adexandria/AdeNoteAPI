using Automapify.Services.Attributes;
using DocBuilder.Services.Utility.Attributes;
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
        [DocRecord("id")]
        public Guid Id { get; set; }


        /// <summary>
        /// Title of the book
        /// </summary>
        [Excelify("title")]
        [DocRecord("title")]
        public string Title { get; set; }


        /// <summary>
        /// Description of the book
        /// </summary>
        [Excelify("description")]
        [DocRecord("description")]
        public string Description { get; set; }

        /// <summary>
        /// A list of pages
        /// </summary>

        public List<BookPages> BookPages { get; set; }
    }
}
