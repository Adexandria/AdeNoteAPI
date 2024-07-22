namespace AdeNote.Models
{
    /// <summary>
    /// A label model
    /// </summary>
    public class Label : BaseEntity
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public Label()
        {
        }
        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="title">Title of the label</param>
        public Label(string title)
        {
            Title = title;
        }

        public void SetModifiedDate()
        {
            Modified = DateTime.UtcNow;
        }
        /// <summary>
        /// Title of the label
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// A list of pages
        /// </summary>
        public IList<Page> Pages { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}