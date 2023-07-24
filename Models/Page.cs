using System.ComponentModel.DataAnnotations.Schema;

namespace AdeNote.Models
{
    public class Page : BaseClass
    {
        public Page()
        {
            Title = "Untitled page";
        }

        public Page(string title)
        {
            Title = title;
        }

        public string Title { get; set; }
        public string Content { get; set; }

        [ForeignKey("BookId")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public IList<Label> Labels { get; set; }
        
    }
}