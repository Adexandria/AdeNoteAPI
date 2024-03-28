using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IUser
    {
        bool IsExist(string email);

        Task<bool> UpdateUser(User currentUser);
    }
}
