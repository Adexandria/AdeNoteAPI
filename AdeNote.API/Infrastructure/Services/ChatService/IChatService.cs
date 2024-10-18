using AdeNote.Infrastructure.Utilities;
using AdeNote.Models.DTOs;

namespace AdeNote.Infrastructure.Services.ChatService
{
    public interface IChatService
    {
        Task<ActionResult> CreateThread(CreateThreadDto message, string userId);

        Task<ActionResult> CreateSubThread(CreateThreadDto message, string userId, string[] replyUserIds, string threadId);

        Task<ActionResult<TweetThreadDto>> GetThread(string threadId);
    }
}
