using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// An interface to perform add,update and remove actions for objects
    /// </summary>
    /// <typeparam name="T">A generic type t</typeparam>
    public interface IRepository<T> where T : IBaseEntity                                                                  
    {
        /// <summary>
        /// Saves a new entity
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        Task<bool> Add(T entity);

        /// <summary>
        /// Updates an existing object
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        Task<bool> Update(T entity);

        /// <summary>
        /// Remove an existing object
        /// </summary>
        /// <param name="entity">A object</param>
        /// <returns>a boolean value</returns>
        Task<bool> Remove(T entity);

    }
}
