using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface ILabelRepository : IRepository<Label>
    {
        Task<Label> GetAsync(Guid id);
        IQueryable<Label> GetAll();
    }
}
