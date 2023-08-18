using AdeNote.Models;
using TasksLibrary.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IAuthRepository : IRepository<UserToken>
    {
        Task<UserToken> GetAuthenticationType(Guid userId);

        Task<UserToken> GetAuthenticationType(string email);

        Task<RefreshToken> GetRefreshTokenByUserId(Guid userId,string refreshToken);

        Task<RefreshToken> GetRefreshToken(string refreshToken);

        Task<bool> RevokeRefreshToken(RefreshToken refreshToken);
    }
}
