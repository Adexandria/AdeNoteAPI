using AdeNote.Models;

namespace AdeNote.Infrastructure.Repository
{
    public interface IRecoveryCodeRepository : IRepository<RecoveryCode>
    {
        public Task<RecoveryCode> GetUserIdByRecoveryCodes(string recoveryCode);

    }
}
