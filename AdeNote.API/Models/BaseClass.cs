using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    /// <summary>
    /// A base class that includes the id
    /// </summary>
    public abstract class BaseClass
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }
    }
}
