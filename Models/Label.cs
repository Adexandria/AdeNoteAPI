namespace AdeNote.Models
{
    public class Label : BaseClass
    {
        public Label()
        {
        }
        public Label(string title)
        {
            Title = title;
        }
        public string Title { get; set; }
        public IList<Page> Pages { get; set; }
    }
}