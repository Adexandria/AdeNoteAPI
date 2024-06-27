using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class RecoveryCodeRepository : Repository<RecoveryCode>,IRecoveryCodeRepository
    {
        public RecoveryCodeRepository(NoteDbContext db, ILoggerFactory loggerFactory): base(db, loggerFactory)
        {
        }

        public async Task<bool> Add(RecoveryCode entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.RecoveryCodes.AddAsync(entity);

            var result =  await SaveChanges();

            logger.LogInformation("Add recovery code to database:{result}", result);

            return result;
        }

        public async Task<RecoveryCode> GetUserIdByRecoveryCodes(string recoveryCode)
        {
            var Code = await Db.RecoveryCodes.Include(s=>s.User).ThenInclude(s => s.RefreshToken).Where(s => s.Codes == recoveryCode).FirstOrDefaultAsync();

            return Code;
        }

        public async Task<bool> Remove(RecoveryCode entity)
        {
            Db.RecoveryCodes.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Remove recovery code to database:{result}", result);

            return result;
        }

        public async Task<bool> Update(RecoveryCode entity)
        {
            var currentCode = await Db.RecoveryCodes
                .FirstOrDefaultAsync(s => s.Id == entity.Id);

            Db.Entry(currentCode).CurrentValues.SetValues(entity);

            Db.Entry(currentCode).State = EntityState.Modified;

            var result = await SaveChanges();

            logger.LogInformation("Update recovery code to database:{result}", result);

            return result;
        }
    }
}
