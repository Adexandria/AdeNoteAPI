namespace AdeNote.Models.DTOs
{
    public class BookDTO
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public IList<BookPages> BookPages { get; set; }
    }
}
