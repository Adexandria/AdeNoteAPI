using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;


namespace AdeNote.Infrastructure.Services
{
    /// <summary>
    /// An interface to interact with the controller
    /// </summary>
    public interface IPageService
    {
        /// <summary>
        /// Adds a new page to a book
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="createPage">A object to create a new page</param>
        /// <returns>An action result</returns>
        Task<ActionResult> Add(Guid bookId,Guid userId,PageCreateDTO createPage);

        /// <summary>
        /// Adds labels to a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="Labels">a list of labels</param>
        /// <returns>An action result</returns>
        Task<ActionResult> AddLabels(Guid bookId, Guid userId,Guid pageId, List<string> Labels);
        /// <summary>
        /// Updates an existing page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="updatePage">An object to update an existing page</param>
        /// <returns>An action result</returns>
        Task<ActionResult> Update(Guid bookId, Guid userId, Guid pageId, PageUpdateDTO updatePage);
        /// <summary>
        /// Removes a particular page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <returns>An action result</returns>
        Task<ActionResult> Remove(Guid bookId, Guid userId, Guid pageId);
        /// <summary>
        /// Gets all pages in a book
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <returns>An action result</returns>
        Task<ActionResult<IEnumerable<PageDTO>>> GetAll(Guid bookId);
        /// <summary>
        /// Get a page by id
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="pageId">page id</param>
        /// <returns>An action result</returns>
        Task<ActionResult<PageDTO>> GetById(Guid bookId, Guid pageId);
        /// <summary>
        /// Removes all labels from a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <returns>An action result</returns>
        Task<ActionResult> RemoveAllPageLabels(Guid bookId, Guid userId,Guid pageId);
        /// <summary>
        /// Removes a particular label from a page
        /// </summary>
        /// <param name="bookId">Book id</param>
        /// <param name="userId">User id</param>
        /// <param name="pageId">Page id</param>
        /// <param name="title">label</param>
        /// <returns>An action result</returns>
        Task<ActionResult> RemovePageLabel(Guid bookId, Guid userId,Guid pageId, string title);


        Task<ActionResult<string>> TranslatePage(Guid bookId, Guid userId, Guid pageId, string translatedLanguage);
    }
}
