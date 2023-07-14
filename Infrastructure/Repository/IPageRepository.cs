using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IPageRepository : IRepository<Page>
    {
        Task<Page> GetBookPage(Guid bookId, Guid pageId);
        IQueryable<Page> GetBookPages(Guid bookId);
    }
}
