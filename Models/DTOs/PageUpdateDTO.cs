namespace AdeNote.Models.DTOs
{
    public class PageUpdateDTO : PageCreateDTO
    {
        public List<string> Labels { get; set; }
    }
}
