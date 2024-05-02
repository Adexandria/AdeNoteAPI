using System.ComponentModel.DataAnnotations.Schema;

namespace AdeNote.Models
{
    /// <summary>
    /// Page model
    /// </summary>
    public class Page : BaseClass
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public Page()
        {
            Title = "Untitled page";
            Labels = new List<Label>();
        }


        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="title">The title of the page</param>
        public Page(string title)
        {
            Title = title;
            Labels = new List<Label>();
        }

        /// <summary>
        /// A title of the page
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Content of the page
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Id of the book
        /// </summary>
        [ForeignKey("Book")]
        public Guid BookId { get; set; }
        
        /// <summary>
        /// A book model
        /// </summary>
        public Book Book { get; set; }
        
        /// <summary>
        /// A list of labels
        /// </summary>
        public IList<Label> Labels { get; set; }
        
    }
}