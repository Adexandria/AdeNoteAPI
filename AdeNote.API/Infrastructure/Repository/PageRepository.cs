using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Handles persisting and querying for page object
    /// </summary>
    public class PageRepository : Repository<Page>, IPageRepository
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
        public PageRepository(NoteDbContext noteDb, ILoggerFactory loggerFactory) : base(noteDb, loggerFactory)
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

            var result = await SaveChanges();

            logger.LogInformation("Add page to database: {result}", result);

            return result;
        }

        /// <summary>
        /// Gets an individual page from a particular book
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="pageId">A page id</param>
        /// <returns>a page object</returns>
        public async Task<Page> GetBookPage(Guid bookId, Guid pageId, bool isTracked)
        {
            var page = Db.Pages.Include(s => s.Book).Include(s=>s.Videos)
                .Include(s => s.Labels);

            if(!isTracked) 
            {
                return await page.AsNoTracking().FirstOrDefaultAsync();
            }

            return await page
                .FirstOrDefaultAsync(s => s.Id == pageId && s.BookId == bookId);
        }

        /// <summary>
        /// Gets all pages from a book
        /// </summary>
        /// <param name="bookId">a book id</param>
        /// <returns>A list of pages</returns>
        public IQueryable<Page> GetBookPages(Guid bookId)
        {
            return Db.Pages.Include(s => s.Book).Include(s => s.Videos)
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

            var result = await SaveChanges();

            logger.LogInformation("Remove page to database: {result}", result);

            return result;
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

            var result = await SaveChanges();

            logger.LogInformation("Update page to database: {result}", result);

            return result;
        }

        /// <summary>
        /// Updates an existing page
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Update(Page entity, Page currentPage)
        {
            Db.Entry(currentPage).CurrentValues.SetValues(entity);

            Db.Entry(currentPage).State = EntityState.Modified;

            var result = await SaveChanges();

            logger.LogInformation("Update page to database: {result}", result);

            return result;
        }
    }
}
