using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services.ChatService
{
    public interface IChatService
    {
        Task<ActionResult> CreateThread(CreateThreadDto message, string email);

        Task<ActionResult> CreateSubThread(CreateThreadDto message, string email, string[] replyEmails, string threadId);

        Task<ActionResult<TweetThreadDto>> GetThread(string threadId);

        Task<ActionResult<TweetThreadDtos>> GetSingleThread(string threadId);

        Task<ActionResult<List<TweetThreadDtos>>> GetThreads();
        Task<ActionResult<List<TweetThreadDtos>>> SearchThreadsByMessage(string message);

        Task<ActionResult<List<TweetThreadDtos>>> SearchThreadsByEmail(string email);

        Task<ActionResult<SubThreadDto>> GetSubThread(string subThreadId, string parentId);

        Task<ActionResult> UpdateThread(string threadId, UpdateThreadDto updateThread);

        Task<ActionResult> UpdateSubThread(string subThreadId, string parentId, UpdateThreadDto updateThread);

        Task<ActionResult> DeleteThread(string threadId);

        Task<ActionResult> DeleteSubThread(string subThreadId, string parentId);

    }
}
