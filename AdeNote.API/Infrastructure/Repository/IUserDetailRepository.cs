using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IUserDetailRepository : IRepository<UserDetail>
    {
        Task<UserDetail> GetUserDetail(Guid userId);
    }
}
