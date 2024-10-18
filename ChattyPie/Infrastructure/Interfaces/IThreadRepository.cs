using ChattyPie.Models.DTOs;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Infrastructure.Interfaces
{
    public interface IThreadRepository : IRepository<Thread>
    {
        Task<ThreadDto> GetThread(string threadId);
        Task<bool> Delete(string threadId);
    }
}
