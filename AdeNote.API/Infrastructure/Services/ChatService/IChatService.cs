using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services.ChatService
{
    public interface IChatService
    {
        Task<ActionResult> CreateThread(CreateThreadDto message, string userId);

        Task<ActionResult> CreateSubThread(CreateThreadDto message, string userId, string[] replyUserIds, string threadId);

        Task<ActionResult<TweetThreadDto>> GetThread(string threadId);

        Task<ActionResult<List<TweetThreadDtos>>> GetThreads();

        Task<ActionResult<SubThreadDto>> GetSubThread(string subThreadId, string parentId);

        Task<ActionResult> UpdateThread(string threadId, UpdateThreadDto updateThread);

        Task<ActionResult> UpdateSubThread(string subThreadId, string parentId, UpdateThreadDto updateThread);

        Task<ActionResult> DeleteThread(string threadId);

        Task<ActionResult> DeleteSubThread(string subThreadId, string parentId);

    }
}
