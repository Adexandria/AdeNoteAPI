using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class BookRepository : Repository, IBookRepository
    {
        public BookRepository(NoteDbContext noteDb) : base(noteDb)
        {
        }

        public async Task<bool> Add(Book entity)
        {
           entity.Id = Guid.NewGuid();
           await _db.Books.AddAsync(entity);
           return await SaveChanges<Book>();
        }

        public IQueryable<Book> GetAll(Guid userId)
        {
            return _db.Books.Where(s=>s.UserId == userId)
                .Include(s=>s.Pages)
                .Include(s=>s.User)
                .AsNoTracking();
        }

        public async Task<Book> GetAsync(Guid bookId, Guid userId)
        {
            return await _db.Books
                .Include(s => s.Pages)
                .Include(s => s.User).AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == bookId && s.UserId == userId);
        }

        public async Task<bool> Remove(Book entity)
        {
            _db.Books.Remove(entity);
            return await SaveChanges<Book>();
        }

        public async Task<bool> Update(Book entity)
        {
            var currentBook = await _db.Books
                .FirstOrDefaultAsync(s => s.Id == entity.Id && s.UserId == entity.UserId);

            _db.Entry(currentBook).CurrentValues.SetValues(entity);

            _db.Entry(currentBook).State = EntityState.Modified;

            return await SaveChanges<Book>();

        }
    }
}
