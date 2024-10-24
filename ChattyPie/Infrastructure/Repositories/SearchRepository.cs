using ChattyPie.Infrastructure.Interfaces;
using ChattyPie.Models.DTOs;

namespace ChattyPie.Infrastructure.Repositories
{
    internal class SearchRepository : ISearchRepository
    {
        public SearchRepository(ThreadQuery _query)
        {
            query = _query;
        }
        public Task<List<ThreadDto>> SearchThreadByMessage(string message)
        {
            return query.SearchThreadsByMessage(message);
        }

        public Task<List<ThreadDto>> SearchThreadByUserId(string userId)
        {
            return query.SearchThreadsByUserId(userId);
        }

        private readonly ThreadQuery query;
    }
}
