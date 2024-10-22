using ChattyPie.Models.DTOs;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Infrastructure.Interfaces
{
    public interface IThreadRepository
    {
        Task<List<ThreadDto>> GetThreads();
        Task<ThreadDtos> GetThread(string threadId);
        Task<Thread> GetSingleThread(string threadId);
        Task<bool> Delete(string threadId);

        Task<ThreadDtos> Add(Thread thread);

        Task<ThreadDtos> Update(Thread thread);
    }
}
