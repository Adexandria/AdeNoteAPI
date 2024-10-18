using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Application
{
    internal class ChatApplication : IChatApplication
    {
        public ChatApplication(IThreadRepository threadRepository, ISubThreadRepository subThreadRepository)
        { 
           _threadRepository = threadRepository;
           _subThreadRepository = subThreadRepository;
        }

        public async Task<bool> CreateThread(Thread newThread)
        {
            return await _threadRepository.Add(newThread);
        }

        public async Task<ThreadDto> FetchThread(string threadId)
        {
            return await _threadRepository.GetThread(threadId);
        }

        public async Task<bool> CreateSubThread(SubThread newSubThread)
        {
            return await _subThreadRepository.Add(newSubThread);
        }

        private readonly ISubThreadRepository _subThreadRepository;
        private readonly IThreadRepository _threadRepository;
    }
}
