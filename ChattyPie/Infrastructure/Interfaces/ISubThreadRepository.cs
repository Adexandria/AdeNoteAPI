using ChattyPie.Models;

namespace ChattyPie.Infrastructure.Interfaces
{
    public interface ISubThreadRepository : IRepository<SubThread>
    {
        Task<bool> Delete(string threadId, string parentThreadId);
    }
}
