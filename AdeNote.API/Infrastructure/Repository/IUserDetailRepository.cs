using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// An interface that includes the details of a user
    /// </summary>
    public interface IUserDetailRepository : IRepository<UserDetail>
    {
        /// <summary>
        /// Get existings of a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User detail object</returns>
        Task<UserDetail> GetUserDetail(Guid userId);
    }
}
