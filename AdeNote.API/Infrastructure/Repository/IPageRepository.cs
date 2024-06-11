using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// An interface to handles  page object
    /// </summary>
    public interface IPageRepository : IRepository<Page>
    {
        /// <summary>
        /// Gets an individual page from a particular book
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <returns>a page object</returns>
        Task<Page> GetBookPage(Guid bookId, Guid pageId,bool isTracked = false);

        /// <summary>
        /// Gets all pages from a book
        /// </summary>
        /// <param name="bookId">a book id</param>
        /// <returns>A list of pages</returns>
        IQueryable<Page> GetBookPages(Guid bookId);

        Task<bool> Update(Page entity, Page currentPage);
    }
}
