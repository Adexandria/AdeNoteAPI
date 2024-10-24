using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Application
{
    public interface IChatApplication
    {
        Task<ThreadDtos> CreateThread(Thread newThread);

        Task<SubThreadDtos> CreateSubThread(SubThread newSubThread);

        Task<ThreadDtos> FetchThread(string threadId);

        Task<Thread> FetchSingleThread(string threadId);

        Task<SubThread> FetchSingleSubThread(string subThreadId, string parentId);

        Task<SubThreadDtos> FetchSubThread(string subThreadId, string parentId);

        Task<List<ThreadDto>> FetchAllParentThread();

        Task<bool> DeleteThread(string threadId);

        Task<bool> DeleteSubThread(string subThreadId, string parentId);

        Task<ThreadDtos> UpdateThread(Thread thread);

        Task<SubThreadDtos> UpdateSubThread(SubThread subThread);

        Task<List<ThreadDto>> SearchThreadsByMessage(string message);

        Task<List<ThreadDto>> SearchThreadsByUserId(string userId);
    }
}
