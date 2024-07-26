using System.ComponentModel.DataAnnotations;

namespace AdeNote.Models
{
    public interface IBaseEntity
    {
        /// <summary>
        /// Id of the entity
        /// </summary>
        public Guid Id { get; set; }
    }
}