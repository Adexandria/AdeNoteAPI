using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    public class LabelPage
    {
        public LabelPage()
        {

        }
        public LabelPage(Guid pageId, Guid labelId)
        {
            PageId = pageId;
            LabelId = labelId;
        }
        public Guid LabelId { get; set; }
        public Guid PageId { get; set; }
    }
}
