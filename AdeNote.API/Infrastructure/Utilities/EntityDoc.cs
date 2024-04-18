using AdeNote.Models.DTOs;
using DocBuilder.Models;

namespace AdeNote.Infrastructure.Utilities
{
    public class EntityDoc<T> : IEntityDoc<T> where T : class
    {
        public IList<T> Entities { get; set; }
        public string Name { get; set; }
    }
}
