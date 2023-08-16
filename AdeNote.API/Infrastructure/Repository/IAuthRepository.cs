using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IAuthRepository : IRepository<UserToken>
    {
        Task<UserToken> GetAuthenticationType(Guid userId);

        Task<UserToken> GetAuthenticationType(string email);
    }
}
