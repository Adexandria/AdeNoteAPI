using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    /// <summary>
    /// A base class that includes the id
    /// </summary>
    public abstract class BaseClass : BaseEntity
    {
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}
