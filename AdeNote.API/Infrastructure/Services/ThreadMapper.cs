using AdeNote.Infrastructure.Repository;
using AdeNote.Models.DTOs;
using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using Thread = ChattyPie.Models.Thread;

namespace AdeNote.Infrastructure.Services
{
    public class ThreadMapper
    {
        public ThreadMapper(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public TweetThreadDto MapTo(ThreadDtos thread)
        {
            var tweetThreadDto = new TweetThreadDto() 
            {
                 Id = new Guid(thread.Id),
                 Message = thread.Message,
                 UserNames = _userRepository.GetUserEmailsByUserIds(thread.UserIds),
                 Date = thread.Date.ToLongDateString(),
                 LastModified = thread.LastModified.ToLongDateString(),
                 Comments = MapTo(thread.SubThreads)
            };

            return tweetThreadDto;
        }

        public TweetThreadDtos MapTo(Thread thread)
        {
            var twitterThreadDto = new TweetThreadDtos()
            {
                Id = new Guid(thread.Id),
                Message = thread.Message,
                UserNames = _userRepository.GetUserEmailsByUserIds(thread.UserIds),
                Date = thread.Created.ToLongDateString(),
                LastModified = thread.Modified.ToLongDateString()
            };

            return twitterThreadDto;
        }

        public TweetThreadDtos MapToTweet(ThreadDtos thread)
        {
            var twitterThreadDto = new TweetThreadDtos()
            {
                Id = new Guid(thread.Id),
                Message = thread.Message,
                UserNames = _userRepository.GetUserEmailsByUserIds(thread.UserIds),
                Date = thread.Date.ToLongDateString(),
                LastModified = thread.LastModified.ToLongDateString()
            };

            return twitterThreadDto;
        }

        public List<TweetThreadDtos> MapTo(List<ThreadDto> threads)
        {
            var threadDtos = threads.Select(s => new TweetThreadDtos()
            {
                Id = new Guid(s.Id),
                Message = s.Message,
                UserNames = _userRepository.GetUserEmailsByUserIds(s.UserIds),
                Date = s.Date.ToLongDateString(),
                LastModified = s.LastModified.ToLongDateString()
            }).ToList();

            return threadDtos;
        }

        public List<TweetThreadDtos> MapTo(List<ThreadDtos> threads)
        {
            var threadDtos = threads.Select(s => new TweetThreadDtos()
            {
                Id = new Guid(s.Id),
                Message = s.Message,
                UserNames = _userRepository.GetUserEmailsByUserIds(s.UserIds),
                Date = s.Date.ToLongDateString(),
                LastModified = s.LastModified.ToLongDateString()
            }).ToList();

            return threadDtos;
        }


        public SubThreadDto MapTo(SubThreadDtos subThread)
        {
            var subThreadDto = new SubThreadDto()
            {
                Message = subThread.Message,
                Id = new Guid(subThread.Id),
                ReplyUsernames = _userRepository.GetUserEmailsByUserIds(subThread.SubUserIds),
                Usernames = _userRepository.GetUserEmailsByUserIds(subThread.UserIds),
                Comments = MapTo(subThread.SubThreads),
                Date = subThread.Date.ToLongDateString(),
                LastModified = subThread.LastModified.ToLongDateString()
            };

            return subThreadDto;
        }

        public Thread MapToThread(ThreadDtos threadDtos)
        {
            var thread = new Thread(threadDtos.UserIds, threadDtos.Message)
            {
                Id = threadDtos.Id,
                Created = threadDtos.Date
            };

            return thread;
        }

        public SubThread MapToSubThread(ThreadDtos threadDtos, string subThreadId)
        {
            var subThreadDto = threadDtos.SubThreads.FirstOrDefault(s => s.Id == subThreadId);

            var subThread = new SubThread(subThreadDto.ThreadId, subThreadDto.UserIds, subThreadDto.SubUserIds, subThreadDto.ThreadId)
            {
                Id = subThreadDto.Id,
                Created = subThreadDto.Date
            };

            return subThread;
        }

        private List<SubThreadDto> MapTo(List<SubThreadDtos> subThreads)
        {
            if(subThreads == null)
            {
                return default;
            }

            var subThreadDto = subThreads.Select(x => new SubThreadDto() 
            { 
                Message = x.Message,
                Id = new Guid(x.Id),
                ReplyUsernames = _userRepository.GetUserEmailsByUserIds(x.SubUserIds),
                Usernames = _userRepository.GetUserEmailsByUserIds(x.UserIds),
                Comments = MapTo(x.SubThreads),
                Date = x.Date.ToLongDateString(),
                LastModified = x.Date.ToLongDateString()
            }).ToList();

            return subThreadDto;
        }

        

        private readonly IUserRepository _userRepository;
    }
}
