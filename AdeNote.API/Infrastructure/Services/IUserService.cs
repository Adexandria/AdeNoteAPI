using AdeNote.Models.DTOs;
using TasksLibrary.Utilities;

namespace AdeNote.Infrastructure.Services
{
    public interface IUserService
    {
        Task<ActionResult<UserDTO>> FetchUserById(Guid userId);

        Task<ActionResult<Guid>> GetUserByEmail(string email);
    }
}
