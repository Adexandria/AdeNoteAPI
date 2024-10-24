using ChattyPie.Models.DTOs;

namespace ChattyPie.Infrastructure.Interfaces
{
    internal interface ISearchRepository
    {
        Task<List<ThreadDto>> SearchThreadByMessage(string message);
        Task<List<ThreadDto>> SearchThreadByUserId(string userId);
    }
}
