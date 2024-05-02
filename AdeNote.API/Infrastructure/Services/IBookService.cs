using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services
{
    /// <summary>
    /// An interface that interacts with the controller
    /// </summary>
    public interface IBookService 
    { 
        /// <summary>
        /// Adds a new book
        /// </summary>
        /// <param name="userId">a user id</param>
        /// <param name="createBook">an object to add a new book</param>
        /// <returns>A custom action result</returns>
        Task<ActionResult> Add(Guid userId,BookCreateDTO createBook);


        Task<ActionResult> Add(Guid userId, IList<BookCreateDTO> newBooks);

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">a user id</param>
        /// <param name="updateBook">An object to update a book</param>
        /// <returns>A custom action result</returns>
        Task<ActionResult> Update(Guid bookId,Guid userId,BookUpdateDTO updateBook);

        /// <summary>
        /// Deletes an existing book
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">a user id</param>
        /// <returns>A custom action result</returns>
        Task<ActionResult> Remove(Guid bookId,Guid userId);

        /// <summary>
        /// Get all books
        /// </summary>
        /// <param name="userId">a user id</param>
        /// <returns>A custom action result</returns>
        Task<ActionResult<IEnumerable<BookDTO>>> GetAll(Guid userId);

        /// <summary>
        /// Get book by id
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">a user id</param>
        /// <returns>A custom action result</returns>
        Task<ActionResult<BookDTO>> GetById(Guid bookId,Guid userId);
    }
}
