using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    /// <summary>
    /// A base class that includes the id
    /// </summary>
    public class BaseClass : IBaseEntity
    {
        public void SetModifiedDate()
        {
            Modified = DateTime.UtcNow;
        }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        [Key]
        public Guid Id { get; set; }
    }
}
