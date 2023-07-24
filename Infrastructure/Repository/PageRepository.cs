using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class PageRepository : Repository, IPageRepository
    {
        public PageRepository(NoteDbContext noteDb) : base(noteDb)
        {
        }

        public async Task<bool> Add(Page entity)
        {
            entity.Id = Guid.NewGuid();
            await _db.Pages.AddAsync(entity);
            return await SaveChanges();
        }


        public async Task<Page> GetBookPage(Guid bookId, Guid pageId)
        {
            return await _db.Pages.Include(s=>s.Book)
                .Include(s=>s.Labels)
                .FirstOrDefaultAsync(s => s.Id == pageId && s.BookId == bookId);
        }

        public IQueryable<Page> GetBookPages(Guid bookId)
        {
            return _db.Pages.Include(s => s.Book)
                .Include(s => s.Labels).Where(s=>s.BookId == bookId);
        }

        public async Task<bool> Remove(Page entity)
        {
            _db.Pages.Remove(entity);
            return await SaveChanges();
        }

        public async Task<bool> Update(Page entity)
        {
            var currentPage = await GetBookPage(entity.BookId,entity.Id);

            _db.Entry(currentPage).CurrentValues.SetValues(entity);

            _db.Entry(currentPage).State = EntityState.Modified;

            return await SaveChanges();
        }
    }
}
