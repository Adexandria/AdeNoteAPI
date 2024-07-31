using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;


namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Handles the persisting and querying of Book objects
    /// </summary>
    public class BookRepository : Repository<Book>, IBookRepository
    {
        /// <summary>
        /// A Constructor
        /// </summary>
        public BookRepository()
        {

        }
        /// <summary>
        /// A Constructor
        /// </summary>
        /// <param name="noteDb">Handles the transactions</param>
        public BookRepository(NoteDbContext noteDb,ILoggerFactory loggerFactory) : base(noteDb,loggerFactory)
        {
        }

        /// <summary>
        /// Saves a new book
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Add(Book entity)
        {
            entity.Id = Guid.NewGuid();
           
            await Db.Books.AddAsync(entity);
           
            var result =  await SaveChanges();
         
            logger.LogInformation("Add book to database: {result}", result);
           
            return result;
        }

        public async Task<bool> Add(IEnumerable<Book> books)
        {
            await Db.Books.AddRangeAsync(books);

            var result = await SaveChanges();

            logger.LogInformation("Add books to database: {result}", result);

            return result;
        }

        /// <summary>
        /// Get all books that belong to a user
        /// </summary>
        /// <param name="userId">A user id</param>
        /// <returns>A list of books</returns>
        public IQueryable<Book> GetAll(Guid userId)
        {
            return Db.Books.Where(s=>s.UserId == userId)
                .Include(s=>s.Pages)
                .AsNoTracking();
        }

        /// <summary>
        /// Get a particular book that belongs to a user
        /// </summary>
        /// <param name="bookId">A book id</param>
        /// <param name="userId">A user id</param>
        /// <returns>Book object</returns>
        public async Task<Book> GetAsync(Guid bookId, Guid userId, bool isTracked)
        {
            var book = Db.Books.Where(s => s.UserId == userId)
                .Include(s => s.Pages);

            if(!isTracked)
            {
                return await book.AsNoTracking().FirstOrDefaultAsync();
            }

            return await book.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Remove an existing book
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Remove(Book entity)
        {
            Db.Books.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Delete book to database: {result}", result);

            return result;
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Update(Book entity)
        {
            var currentBook = await Db.Books
                .FirstOrDefaultAsync(s => s.Id == entity.Id && s.UserId == entity.UserId);

            Db.Entry(currentBook).CurrentValues.SetValues(entity);

            Db.Entry(currentBook).State = EntityState.Modified;

            var result = await SaveChanges();

            logger.LogInformation("Update book to database: {result}", result);

            return result;
        }

        /// <summary>
        /// Updates an existing book
        /// </summary>
        /// <param name="entity">A book object</param>
        /// <returns>a boolean value</returns>
        public async Task<bool> Update(Book entity, Book currentBook)
        {
            Db.Entry(currentBook).CurrentValues.SetValues(entity);

            Db.Entry(currentBook).State = EntityState.Modified;

            var result = await SaveChanges();

            logger.LogInformation("Update book to database: {result}", result);

            return result;
        }
    }
}
