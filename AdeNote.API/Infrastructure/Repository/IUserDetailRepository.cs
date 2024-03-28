using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    /// <summary>
    /// Handles the details of a user
    /// </summary>
    public interface IUserDetailRepository : IRepository<UserDetail>
    {
        /// <summary>
        /// Get existings of a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User detail object</returns>
        Task<UserDetail> GetUserDetail(Guid userId);

        /// <summary>
        /// Checks if user's phone number has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>A boolean value</returns>
        Task<bool> IsPhoneNumberVerified(Guid userId);
    }
}
