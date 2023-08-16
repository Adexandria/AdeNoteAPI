using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IUserRepository
    {
        Task<User> GetUserbyId(Guid userId);
        Task<User> GetUserByEmail(string email);
    }
}
