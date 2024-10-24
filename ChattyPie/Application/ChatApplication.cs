using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Models;
using ChattyPie.Models.DTOs;
using Thread = ChattyPie.Models.Thread;

namespace ChattyPie.Application
{
    internal class ChatApplication : IChatApplication
    {
        public ChatApplication(IThreadRepository threadRepository, 
            ISearchRepository searchRepository,
            ISubThreadRepository subThreadRepository)
        { 
           _threadRepository = threadRepository;
            _searchRepository = searchRepository;
           _subThreadRepository = subThreadRepository;
        }

        public async Task<ThreadDtos> CreateThread(Thread newThread)
        {
            return await _threadRepository.Add(newThread);
        }

        public async Task<ThreadDtos> FetchThread(string threadId)
        {
            return await _threadRepository.GetThread(threadId);
        }

        public async Task<SubThreadDtos> CreateSubThread(SubThread newSubThread)
        {
            return await _subThreadRepository.Add(newSubThread);
        }

        public async Task<SubThreadDtos> FetchSubThread(string subThreadId, string parentId)
        {
           return await _subThreadRepository.GetSubThreadAsync(subThreadId, parentId);
        }

        public async Task<List<ThreadDto>> FetchAllParentThread()
        {
            return await _threadRepository.GetThreads();
        }

        public async Task<List<ThreadDto>> SearchThreadsByMessage(string message)
        {
            return await _searchRepository.SearchThreadByMessage(message);
        }

        public async Task<List<ThreadDto>> SearchThreadsByUserId(string userId)
        {
            return await _searchRepository.SearchThreadByUserId(userId);
        }
        public async Task<bool> DeleteThread(string threadId)
        {
           return await _threadRepository.Delete(threadId);
        }

        public async Task<bool> DeleteSubThread(string subThreadId, string parentId)
        {
           return await _subThreadRepository.Delete(subThreadId, parentId);
        }

        public async Task<ThreadDtos> UpdateThread(Thread thread)
        {
           return await _threadRepository.Update(thread);
        }

        public async Task<SubThreadDtos> UpdateSubThread(SubThread subThread)
        {
           return await _subThreadRepository.Update(subThread);
        }

        public async Task<Thread> FetchSingleThread(string threadId)
        {
            return await _threadRepository.GetSingleThread(threadId);
        }

        public async Task<SubThread> FetchSingleSubThread(string subThreadId, string parentId)
        {
            return await _subThreadRepository.GetSingleSubThread(subThreadId, parentId); 
        }

        private readonly ISubThreadRepository _subThreadRepository;
        private readonly ISearchRepository _searchRepository;
        private readonly IThreadRepository _threadRepository;
    }
}
