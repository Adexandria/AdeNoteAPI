using System.ComponentModel.DataAnnotations;

namespace AdeAuth.Models
{
    internal class BaseEntity
    {
        public BaseEntity()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
    }
}
