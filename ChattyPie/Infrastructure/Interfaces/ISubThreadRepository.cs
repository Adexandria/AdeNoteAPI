using ChattyPie.Models;
using ChattyPie.Models.DTOs;

namespace ChattyPie.Infrastructure.Interfaces
{
    public interface ISubThreadRepository
    {
        Task<SubThreadDtos> GetSubThreadAsync(string threadId, string parentThreadId);
        Task<SubThread> GetSingleSubThread(string subThreadId, string parentThreadId);
        Task<bool> Delete(string threadId, string parentThreadId);
        Task<SubThreadDtos> Add(SubThread thread);
        Task<SubThreadDtos> Update(SubThread thread);
    }
}
