using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IUser
    {
        Task<bool> UpdateUser(User currentUser);
    }
}
