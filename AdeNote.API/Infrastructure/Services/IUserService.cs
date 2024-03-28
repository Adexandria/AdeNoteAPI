using TasksLibrary.Models;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IUserService
    {
        Task<ActionResult> ResetUserPassword(Guid userId, string password);

        Task<ActionResult> UpdateUserPassword(Guid userId, string currentPassword, string password);

        Task<ActionResult<User>> GetUser(string email);

        Task<ActionResult> IsUserExist(string email);
    }
}
