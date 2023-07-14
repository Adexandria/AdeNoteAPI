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

        public Page AddLabels(List<string> labels)
        {
            foreach (var item in labels)
            {
                Labels.Add(new Label(item));
            }
            return this;
        }
        public string Title { get; set; }
        public string Content { get; set; }

        [ForeignKey("BookId")]
        public Guid BookId { get; set; }
        public Book Book { get; set; }
        public IList<Label> Labels { get; set; }
        
    }
}