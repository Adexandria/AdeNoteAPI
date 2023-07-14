using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IBookRepository : IRepository<Book>
    {
        Task<Book> GetAsync(Guid bookId, Guid userId);
        IQueryable<Book> GetAll(Guid userId);
    }
}
