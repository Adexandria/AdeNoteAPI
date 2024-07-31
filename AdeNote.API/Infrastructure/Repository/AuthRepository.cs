using AdeAuth.Services;
using AdeAuth.Services.Interfaces;
using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Infrastructure.Repository
{
    public class AuthRepository : IdentityService<User>
    {
        public AuthRepository(IdentityDbContext identityDb, IPasswordManager _passwordManager)
        {
            _db = identityDb;
            passwordManager = _passwordManager;
        }
        public override async Task<User> AuthenticateUsingEmailAsync(string email, string password)
        {
            var user = await _db.Users.
                  AsNoTracking()
                  .Include(s => s.RefreshToken)
                  .Include(s => s.RecoveryCode)
                  .Where(s => s.Email == email)
                  .FirstOrDefaultAsync();

            if (user != null && user.AuthenticationType == AuthType.local)
            {
                var isVerified = passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);
                if (isVerified)
                {
                    return user;
                }
            }

            return user;
        }

        public override async Task<User> AuthenticateUsingUsernameAsync(string username, string password)
        {
            var user = await _db.Users.
                AsNoTracking()
                .Include(s => s.RefreshToken)
                .Include(s => s.RecoveryCode)
                .Where(s => s.UserName == username)
                .FirstOrDefaultAsync();

            if (user != null && user.AuthenticationType == AuthType.local)
            {
                var isVerified = passwordManager.VerifyPassword(password, user.PasswordHash, user.Salt);
                if (isVerified)
                {
                    return user;
                }
            }

            return user;
        }

        public override async Task<bool> CreateUserAsync(User user)
        {

            await _db.Users.AddAsync(user);

            var result = await SaveChanges();

            return result;
        }

        public virtual async Task<bool> SaveChanges()
        {

            var saved = false;
            while (!saved)
            {
                try
                {
                    int commitedResult = await _db.SaveChangesAsync();
                    if (commitedResult == 0)
                    {
                        saved = false;
                        break;
                    }
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is User)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var databaseValue = databaseValues[property];
                            }

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }
            return saved;

        }


        private readonly IdentityDbContext _db;
        private readonly IPasswordManager passwordManager;
    }
}
