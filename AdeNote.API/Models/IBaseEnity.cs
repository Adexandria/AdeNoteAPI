using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    public class BaseEntity
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        [Key]
        public Guid Id { get; set; }
    }
}