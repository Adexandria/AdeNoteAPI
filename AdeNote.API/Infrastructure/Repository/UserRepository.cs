using AdeAuth.Services.Interfaces;
using AdeNote.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;
using NPOI.SS.Formula.Functions;

namespace AdeNote.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(IdentityDbContext dbContext,
            IPasswordManager passwordManager,
            ILoggerFactory loggerFactory)
        {
            PasswordManager = passwordManager;
            logger = loggerFactory.CreateLogger<UserRepository>();
            Db = dbContext;
        }

        public async Task<bool> Add(User entity)
        {
            entity.Id = Guid.NewGuid();

            await Db.Users.AddAsync(entity);

            var result = await SaveChanges();

            logger.LogInformation("Add user to database:{result}", result);

            return result;
        }

        public async Task<User> AuthenticateUser(string email, string password, AuthType authType)
        {
            var user = await Db.Users.
                   AsNoTracking()
                   .Include(s=>s.RefreshToken)
                   .Include(s => s.RecoveryCode)
                   .Where(s => s.Email == email)
                   .FirstOrDefaultAsync();

            if(authType != user.AuthenticationType)
            {
                return default;
            }

            if (user != null && user.AuthenticationType == AuthType.local)
            {
                var isVerified = PasswordManager.VerifyPassword(password, user.PasswordHash, user.Salt);
                if (isVerified)
                {
                    return user;
                }
            }

            return user;
        }

        public async Task<User> GetUserByEmail(string email)
        {
            var user = await Db.Users
                .Include(s=>s.RefreshToken).Include(s=>s.RecoveryCode).Where(s => s.Email == email).FirstOrDefaultAsync();

            return user;
        }

        public async Task<User> GetUser(Guid userId)
        {
            return await Db.Users.Include(s => s.RecoveryCode)
              .Include(s=>s.RefreshToken)
              .FirstOrDefaultAsync(x => x.Id == userId);
        }


        public async Task<bool> IsPhoneNumberVerified(Guid userId)
        {
            var isPhoneNumberVerified = await Db.Users.
                  AsNoTracking().Where(s => s.Id == userId)
                  .Select(s => s.PhoneNumberConfirmed).FirstOrDefaultAsync();

            return isPhoneNumberVerified;
        }

        public async Task<bool> Remove(User entity)
        {
            Db.Users.Remove(entity);

            var result = await SaveChanges();

            logger.LogInformation("Remove user to database:{result}", result);

            return result;
        }

        public async Task<bool> Update(User entity)
        {
            Db.Users.Update(entity);

            var result = await SaveChanges();

            logger.LogInformation("Update user to database:{result}", result);

            return result;
        }


        public bool IsExist(string email)
        {
            return Db.Users.Any(s => s.Email == email);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    
        }

        public int GetNumberOfUsers()
        {
            var noOfUsers = Db.Users.Where(s => s.Role == Role.User).Count();

            return noOfUsers;
        }


        public virtual async Task<bool> SaveChanges()
        {

            var saved = false;
            while (!saved)
            {
                try
                {
                    int commitedResult = await Db.SaveChangesAsync();
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
                        if (entry.Entity is T)
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
        private readonly IdentityDbContext Db;

        private readonly ILogger<UserRepository> logger;

        public readonly IPasswordManager PasswordManager; 
    }
}
