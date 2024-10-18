
namespace AdeNote.Models
{
    public class Video: IBaseEntity
    {
        public Video(string description, Guid pageId, string fileName)
        {
            Description = description;
            Created = DateTime.Now;
            Modified = DateTime.Now;
            PageId = pageId;
            FileName = fileName;

        }

        public Video(string description, string transcript, Guid pageId, string fileName)
        {
            Description = description;
            Transcript = transcript;
            Created = DateTime.Now;
            Modified = DateTime.Now;
            PageId = pageId;
            FileName = fileName;
        }

        public void SetModifiedDate()
        {
            Modified = DateTime.Now;
        }

        public Guid Id { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string Transcript { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public Guid PageId { get; set; }
        public Page Page { get; set; }
    }
}
