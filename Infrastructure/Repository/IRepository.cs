using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IRepository<T> where T : BaseClass
    {
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Remove(T entity);

    }
}
