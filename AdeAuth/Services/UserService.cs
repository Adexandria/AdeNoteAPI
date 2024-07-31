using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Services
{
    internal class UserService<TDbContext,TModel> : IdentityService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationUser,new()
    {
        public UserService(TDbContext dbContext,
            IPasswordManager passwordManager)
        {
             Db = dbContext;
            _users = dbContext.Set<TModel>();
            _passwordManager = passwordManager;
        }

        public override async Task<TModel> AuthenticateUsingUsernameAsync(string username, string password)
        {
            var currentUser = await _users.Where(s=>s.UserName == username).FirstOrDefaultAsync();
            if (currentUser == null)
            {
                return default;
            }

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, currentUser.PasswordHash, currentUser.Salt);

            if (isPasswordCorrect)
            {
                return currentUser;
            }

            return default;
        }
        public override async Task<TModel> AuthenticateUsingEmailAsync(string email, string password)
        {
            var currentUser = await _users.Where(s => s.Email == email).FirstOrDefaultAsync();
            if (currentUser == null)
            {
                return default;
            }

            var isPasswordCorrect = _passwordManager.VerifyPassword(password, currentUser.PasswordHash, currentUser.Salt);

            if (isPasswordCorrect)
            {
                return currentUser;
            }

            return default;
        }

        public override async Task<bool> CreateUserAsync(TModel user)
        {
           _users.Add(user);

           return await SaveChangesAsync();
        }

        private async Task<bool> SaveChangesAsync()
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
                        if (entry.Entity is TModel)
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

        private readonly TDbContext Db;

        private readonly IPasswordManager _passwordManager;

        private readonly DbSet<TModel> _users;
    }
}
