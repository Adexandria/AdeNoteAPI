using AdeCache.Services;
using AdeNote.Infrastructure.Extension;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using ChattyPie.Application;
using ChattyPie.Models.DTOs;
using TweetThreadDto = AdeNote.Models.DTOs.TweetThreadDto;

namespace AdeNote.Infrastructure.Services.ChatService
{
    public class ChatService : IChatService
    {
        public ChatService(IChatApplication chatApplication, 
            ThreadMapper threadMapper, ICacheService cacheService, CachingKeys cachingKeys)
        {
            _chatApplication = chatApplication;
            _threadMapper = threadMapper;
            _cacheService = cacheService;
            _cacheKey = cachingKeys.TweetsCacheKey;
        }
        public async Task<ActionResult> CreateSubThread(CreateThreadDto message, string userId, string[] replyUserIds, string threadId)
        {
           var newThread = new TweetSubThread(threadId, userId, replyUserIds.ToList(), message.Message);
           
            var response = await _chatApplication.CreateThread(newThread);

            if (response)
                return ActionResult.Failed("Failed create tweet", 400);

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult> CreateThread(CreateThreadDto message, string userId)
        {
            var newThread = new TweetThread(userId, message.Message);

            var response = await _chatApplication.CreateThread(newThread);

            if (response)
                return ActionResult.Failed("Failed create tweet", 400);

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult<TweetThreadDto>> GetThread(string threadId)
        {
            var cachedThread = _cacheService.Get<ThreadDto>($"{_cacheKey}:{threadId}");

            if(cachedThread == null)
            {
                cachedThread = await _chatApplication.FetchThread(threadId);

                _cacheService.Set($"{_cacheKey}:{threadId}", cachedThread);
            }

            var threadDto = _threadMapper.MapTo(cachedThread);

            return ActionResult<TweetThreadDto>.SuccessfulOperation(threadDto);
        }

        private readonly ICacheService _cacheService;
        private readonly ThreadMapper _threadMapper;
        private readonly string _cacheKey;
        private readonly IChatApplication _chatApplication;
    }
}
