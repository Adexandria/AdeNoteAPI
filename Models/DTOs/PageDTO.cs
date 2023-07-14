namespace AdeNote.Models.DTOs
{
    public class PageDTO : BookPages
    {
        public string Content { get; set; }
        public IList<LabelDTO> Labels { get; set; }
    }
}
