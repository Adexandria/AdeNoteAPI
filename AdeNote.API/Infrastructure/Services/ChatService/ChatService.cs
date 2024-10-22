using AdeCache.Services;
using AdeNote.Infrastructure.Utilities;
using AdeNote.Models;
using AdeNote.Models.DTOs;
using ChattyPie.Application;
using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using TweetThreadDto = AdeNote.Models.DTOs.TweetThreadDto;
using UpdateThreadDto = AdeNote.Models.DTOs.UpdateThreadDto;

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
            _threadCacheKey = cachingKeys.TweetsCacheKey;
        }
        public async Task<ActionResult> CreateSubThread(CreateThreadDto message, string userId, string[] replyUserIds, string threadId)
        {
           var newThread = new TweetSubThread(threadId, userId, replyUserIds.ToList(), message.Message);
           
            var response = await _chatApplication.CreateSubThread(newThread);

            if (response == null)
                return ActionResult.Failed("Failed create tweet", 400);

            var cachedThread = _cacheService.Get<ThreadDtos>($"{_threadCacheKey}:{threadId}");

            if(cachedThread == null)
            {
                cachedThread = await _chatApplication.FetchThread(threadId);
            }
            else
            {
                cachedThread.SubThreads.Add(response);
            }

            _cacheService.Set($"{_threadCacheKey}:{response.ThreadId}", cachedThread);

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult> CreateThread(CreateThreadDto message, string userId)
        {
            var newThread = new TweetThread(userId, message.Message);

            var response = await _chatApplication.CreateThread(newThread);

            if (response == null)
                return ActionResult.Failed("Failed create tweet", 400);

            _cacheService.Set($"{_threadCacheKey}:{response.Id}", response);

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult<TweetThreadDto>> GetThread(string threadId)
        {
            var cachedThread = _cacheService.Get<ThreadDtos>($"{_threadCacheKey}:{threadId}");

            if(cachedThread == null)
            {
                cachedThread = await _chatApplication.FetchThread(threadId);

                if (cachedThread == null)
                    return ActionResult<TweetThreadDto>.Failed("Tweet does not exist");

                _cacheService.Set($"{_threadCacheKey}:{threadId}", cachedThread, DateTime.UtcNow.AddMinutes(30));
            }

            var threadDto = _threadMapper.MapTo(cachedThread);

            return ActionResult<TweetThreadDto>.SuccessfulOperation(threadDto);
        }

        public async Task<ActionResult<List<TweetThreadDtos>>> GetThreads()
        {
            var cachedThreads = _cacheService.Search<ThreadDtos>(_threadCacheKey, "*");

            List<TweetThreadDtos> tweets = new();
            if (cachedThreads == null)
            {
                var threads = await _chatApplication.FetchAllParentThread();

                if(threads == null)
                    return ActionResult<List<TweetThreadDtos>>.SuccessfulOperation(tweets);

                tweets = _threadMapper.MapTo(threads);
            }
            else
            {
                tweets = _threadMapper.MapTo(cachedThreads.ToList());
            }

            return ActionResult<List<TweetThreadDtos>>.SuccessfulOperation(tweets);
        }


        public async Task<ActionResult<SubThreadDto>> GetSubThread(string subThreadId, string parentId)
        {
            SubThreadDtos cachedSubThread = null;

            var thread = _cacheService.Get<ThreadDtos>($"{_threadCacheKey}:{parentId}");

            if (thread != null)
            {
                cachedSubThread = thread.SubThreads.FirstOrDefault(s => s.Id == subThreadId && s.ThreadId == parentId);
            }
           
            if(cachedSubThread == null)
            {
                cachedSubThread = await _chatApplication.FetchSubThread(subThreadId, parentId);

                if (cachedSubThread == null)
                    return ActionResult<SubThreadDto>.Failed("Tweet does not exist", StatusCodes.Status404NotFound);
            }

            var subThreads = _threadMapper.MapTo(cachedSubThread);

            return ActionResult<SubThreadDto>.SuccessfulOperation(subThreads);
        }


        public async Task<ActionResult> UpdateThread(string threadId, UpdateThreadDto updateThread)
        {
            ChattyPie.Models.Thread thread = null;

            var cachedThread = _cacheService.Get<ThreadDtos>($"{_threadCacheKey}:{threadId}");

            if (cachedThread == null)
            {
                thread = await _chatApplication.FetchSingleThread(threadId);
                if(thread == null) return ActionResult.Failed("Tweet does not exist", StatusCodes.Status404NotFound);
            }
            else
            {
                thread = _threadMapper.MapToThread(cachedThread);
            }

            thread.UpdateMessage(updateThread.Message);

            var response = await _chatApplication.UpdateThread(thread);

            if (response == null)
                return ActionResult.Failed("Failed to update tweet", StatusCodes.Status400BadRequest);

            if (cachedThread == null)
            {
                cachedThread = await _chatApplication.FetchThread(threadId);
            }
            else
            {
                cachedThread.Date = response.Date;
                cachedThread.Message = response.Message;
            }
          

            _cacheService.Set($"{_threadCacheKey}:{threadId}", cachedThread, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult> UpdateSubThread(string subThreadId, string parentId, UpdateThreadDto updateThread)
        {
            SubThread subThread = null;

            var cachedThread = _cacheService.Get<ThreadDtos>($"{_threadCacheKey}:{parentId}");
            
            if(cachedThread == null)
            {
                subThread = await _chatApplication.FetchSingleSubThread(subThreadId, parentId);
            }
            else
            {
                subThread = _threadMapper.MapToSubThread(cachedThread, subThreadId);
            }

            subThread.UpdateMessage(updateThread.Message);

            var response = await _chatApplication.UpdateSubThread(subThread);

            if (response  == null)
                return ActionResult.Failed("Failed to update tweet", StatusCodes.Status400BadRequest);

            if (cachedThread == null)
            {
                cachedThread = await _chatApplication.FetchThread(parentId);
            }
            else
            {
                cachedThread.SubThreads.ForEach(s =>
                {
                    if(s.Id != subThread.Id && s.ThreadId != parentId)
                    {
                        return;
                    }

                    s.Message = response.Message;
                    s.Date = s.Date;
                });
            }
             
            _cacheService.Set($"{_threadCacheKey}:{parentId}", cachedThread, DateTime.UtcNow.AddMinutes(30));

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult> DeleteThread(string threadId)
        {
            var cachedThread = _cacheService.Get<ThreadDtos>($"{_threadCacheKey}:{threadId}");
            if(cachedThread == null)
            {
               var thread =  await _chatApplication.FetchSingleThread(threadId);
                if (thread == null)
                    return ActionResult.Failed("Thread not found", StatusCodes.Status404NotFound);
            }

            var response = await _chatApplication.DeleteThread(threadId);

            if (!response)
                return ActionResult.Failed("Failed to delete tweet", StatusCodes.Status400BadRequest);

            _cacheService.Remove($"{_threadCacheKey}:{threadId}");

            return ActionResult.SuccessfulOperation();
        }

        public async Task<ActionResult> DeleteSubThread(string subThreadId, string parentId)
        {
            SubThreadDtos subThreadDto = null;
            var cachedThread = _cacheService.Get<ThreadDtos>($"{_threadCacheKey}:{parentId}");

            if(cachedThread == null)
            {
                var subThread = await _chatApplication.FetchSingleSubThread(subThreadId, parentId);
                if (subThread == null)
                    return ActionResult.Failed("Thread not found", StatusCodes.Status404NotFound);
            }
            else
            {
                 subThreadDto = cachedThread.SubThreads.FirstOrDefault(s => s.Id == subThreadId);
                if(subThreadDto == null)
                    return ActionResult.Failed("Thread not found", StatusCodes.Status404NotFound);
            }

            var response = await _chatApplication.DeleteSubThread(subThreadId, parentId);

            if (!response)
                return ActionResult.Failed("Failed to delete tweet", StatusCodes.Status400BadRequest);

            if(cachedThread != null)
            {
                cachedThread.SubThreads.Remove(subThreadDto);
                _cacheService.Set($"{_threadCacheKey}:{parentId}",cachedThread, DateTime.UtcNow.AddMinutes(30));
            }

            return ActionResult.SuccessfulOperation();
        }


        private readonly ICacheService _cacheService;
        private readonly ThreadMapper _threadMapper;
        private readonly string _threadCacheKey;
        private readonly IChatApplication _chatApplication;
    }
}
