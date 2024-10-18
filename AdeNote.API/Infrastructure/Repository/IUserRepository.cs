using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User> GetUserByEmail(string email);
        bool IsExist(string email);
        Task<User> AuthenticateUser(string email, string password, AuthType authType);

        /// <summary>
        /// Get existings of a user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>User detail object</returns>
        Task<User> GetUser(Guid userId);

        /// <summary>
        /// Checks if user's phone number has been verified
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns>A boolean value</returns>
        Task<bool> IsPhoneNumberVerified(Guid userId);

        List<string> GetUserEmails(List<string> userIds);

        int GetNumberOfUsers();
    }
}
