using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class RecoveryCodeRepository : Repository,IRecoveryCodeRepository
    {
        public RecoveryCodeRepository(NoteDbContext db): base(db)
        {
            
        }

        public async Task<bool> Add(RecoveryCode entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.RecoveryCodes.AddAsync(entity);

            return await SaveChanges<RecoveryCode>();  
        }

        public async Task<RecoveryCode> GetUserIdByRecoveryCodes(string recoveryCode)
        {
            var Code = await Db.RecoveryCodes.Where(s => s.Codes == recoveryCode).FirstOrDefaultAsync();

            return Code;
        }

        public async Task<bool> Remove(RecoveryCode entity)
        {
            Db.RecoveryCodes.Remove(entity);
            return await SaveChanges<RecoveryCode>();
        }

        public async Task<bool> Update(RecoveryCode entity)
        {
            var currentCode = await Db.RecoveryCodes
                .FirstOrDefaultAsync(s => s.Id == entity.Id);

            Db.Entry(currentCode).CurrentValues.SetValues(entity);

            Db.Entry(currentCode).State = EntityState.Modified;

            return await SaveChanges<RecoveryCode>();
        }
    }
}
