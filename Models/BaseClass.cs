using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    public abstract class BaseClass
    {
        [Key]
        public Guid Id { get; set; }
    }
}
