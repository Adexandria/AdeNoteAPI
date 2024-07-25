using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Services
{
    internal class UserService<TDbContext> : ServiceBase<TDbContext, ApplicationUser>, IUserService<ApplicationUser>
        where TDbContext : DbContext
    {
        public UserService(TDbContext dbContext) : base(dbContext)
        {
            _users = dbContext.Set<ApplicationUser>();
        }

        public bool AddRole(ApplicationUser User, string RoleName)
        {
            
        }

        public bool AuthenticateUsingUsername(string username, string password)
        {

        }
        public bool AuthenticateUsingEmail(string email, string password)
        {

        }

        public bool SignUpUser(ApplicationUser user)
        {
            throw new NotImplementedException();
        }

        private readonly DbSet<UserRole> _roles;
        private readonly DbSet<ApplicationUser> _users;
    }
}
