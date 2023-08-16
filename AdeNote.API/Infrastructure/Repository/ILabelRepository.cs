using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// An interface that includes label object
    /// </summary>
    public interface ILabelRepository : IRepository<Label>
    {
        /// <summary>
        /// Get a particular label
        /// </summary>
        /// <param name="id">label id</param>
        /// <returns>boolean value</returns>
        Task<Label> GetAsync(Guid id);

        /// <summary>
        /// Get all labels
        /// </summary>
        /// <returns>A list of labels</returns>
        IQueryable<Label> GetAll();

        /// <summary>
        /// Gets a label by name
        /// </summary>
        /// <param name="name">label's name</param>
        /// <returns>Label</returns>
        Task<Label> GetByNameAsync(string name);
    }
}
