using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Services
{
    internal class UserService<TDbContext,TModel> : ServiceBase<TDbContext, ApplicationUser>, IUserService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationUser,new()
    {
        public UserService(TDbContext dbContext) : base(dbContext)
        { 
            _users = dbContext.Set<TModel>();
        }

        public TModel AuthenticateUsingUsername(string username, string password)
        {
            var currentUser = _users.Where(s=>s.UserName == username).FirstOrDefault();
            if (currentUser == null)
            {
                return default;
            }

            var isPasswordCorrect = PasswordManager.VerifyPassword(password, currentUser.PasswordHash, currentUser.Salt);

            if (isPasswordCorrect)
            {
                return currentUser;
            }

            return default;
        }
        public TModel AuthenticateUsingEmail(string email, string password)
        {
            var currentUser = _users.Where(s => s.Email == email).FirstOrDefault();
            if (currentUser == null)
            {
                return default;
            }

            var isPasswordCorrect = PasswordManager.VerifyPassword(password, currentUser.PasswordHash, currentUser.Salt);

            if (isPasswordCorrect)
            {
                return currentUser;
            }

            return default;
        }

        public async Task<bool> SignUpUser(TModel user)
        {
           _users.Add(user);

           return await SaveChangesAsync();
        }

        public IPasswordManager PasswordManager { get; set; }
        private readonly DbSet<TModel> _users;
    }
}
