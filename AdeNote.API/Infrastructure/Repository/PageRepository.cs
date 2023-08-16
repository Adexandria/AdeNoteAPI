using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Handles persisting and querying for page object
    /// </summary>
    public class PageRepository : Repository, IPageRepository
    {
        /// <summary>
        /// A constructor
        /// </summary>
        public PageRepository()
        {

        }
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="noteDb">Handles transaction</param>
        public PageRepository(NoteDbContext noteDb) : base(noteDb)
        {
        }

        /// <summary>
        /// Saves a new page
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Add(Page entity)
        {
            entity.Id = Guid.NewGuid();
            await Db.Pages.AddAsync(entity);
            return await SaveChanges<Page>();
        }

        /// <summary>
        /// Gets an individual page from a particular book
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <returns>a page object</returns>
        public async Task<Page> GetBookPage(Guid bookId, Guid pageId)
        {
            return await Db.Pages.Include(s=>s.Book)
                .Include(s=>s.Labels).AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == pageId && s.BookId == bookId);
        }

        /// <summary>
        /// Gets all pages from a book
        /// </summary>
        /// <param name="bookId">a book id</param>
        /// <returns>A list of pages</returns>
        public IQueryable<Page> GetBookPages(Guid bookId)
        {
            return Db.Pages.Include(s => s.Book)
                .Include(s => s.Labels).Where(s=>s.BookId == bookId).AsNoTracking();
        }

        /// <summary>
        /// Remove an existing page
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Remove(Page entity)
        {
            Db.Pages.Remove(entity);
            return await SaveChanges<Page>();
        }

        /// <summary>
        /// Updates an existing page
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Update(Page entity)
        {
            var currentPage = await Db.Pages.Include(s => s.Book)
                .Include(s => s.Labels)
                .FirstOrDefaultAsync(s => s.Id == entity.Id && s.BookId == entity.BookId);

            Db.Entry(currentPage).CurrentValues.SetValues(entity);

            Db.Entry(currentPage).State = EntityState.Modified;

            return await SaveChanges<Page>();
        }
    }
}
