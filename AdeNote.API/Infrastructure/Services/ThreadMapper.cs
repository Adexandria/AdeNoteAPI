using AdeNote.Db;
using AdeNote.Infrastructure.Repository;
using AdeNote.Models.DTOs;
using ChattyPie.Models.DTOs;

namespace AdeNote.Infrastructure.Services
{
    public class ThreadMapper
    {
        public ThreadMapper(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public TweetThreadDto MapTo(ThreadDto thread)
        {
            var tweetThreadDto = new TweetThreadDto() 
            {
                 Id = new Guid(thread.Id),
                 Message = thread.Message,
                 UserNames = _userRepository.GetUserEmails(thread.UserIds),
                 Messages = MapTo(thread.SubThreads)
            };

            return tweetThreadDto;
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
                ReplyUsernames = _userRepository.GetUserEmails(x.SubUserIds),
                Usernames = _userRepository.GetUserEmails(x.UserIds),
                Messages = MapTo(x.SubThreads)
            }).ToList();

            return subThreadDto;
        }

        private readonly IUserRepository _userRepository;
    }
}
