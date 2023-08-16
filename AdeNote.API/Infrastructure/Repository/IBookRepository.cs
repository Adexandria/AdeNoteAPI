using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// An interface that includes the behaviour of a book object
    /// </summary>
    public interface IBookRepository : IRepository<Book>
    {
        /// <summary>
        /// Get a particular book that belongs to a user
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">A user id</param>
        /// <returns>A book object</returns>
       Task<Book> GetAsync(Guid bookId, Guid userId);

        /// <summary>
        /// Get all books that belong to a user
        /// </summary>
        /// <param name="userId">A user id</param>
        /// <returns>a list of books</returns>
        IQueryable<Book> GetAll(Guid userId);
    }
}
