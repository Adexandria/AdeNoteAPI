using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Services
{
    /// <summary>
    /// Manages user service
    /// </summary>
    /// <typeparam name="TDbContext">Context to manage operation</typeparam>
    /// <typeparam name="TModel">Application user</typeparam>
    internal class UserService<TDbContext,TModel> : IdentityService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationUser,new()
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="dbContext">Context to manage operation</param>
        /// <param name="passwordManager">Manages passwords</param>
        public UserService(TDbContext dbContext,
            IPasswordManager passwordManager)
        {
             Db = dbContext;
            _users = dbContext.Set<TModel>();
            _passwordManager = passwordManager;
        }


        /// <summary>
        /// Authenticate user using username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
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


        /// <summary>
        /// Authenticate user using email and password
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
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

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user">New user to create</param>
        /// <returns>Boolean value</returns>
        public override async Task<bool> CreateUserAsync(TModel user)
        {
           _users.Add(user);

           return await SaveChangesAsync();
        }

        /// <summary>
        /// Save changes that support concurrency exception
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
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
