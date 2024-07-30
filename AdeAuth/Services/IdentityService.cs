using AdeAuth.Models;
using AdeAuth.Services.Interfaces;


namespace AdeAuth.Services
{
    public abstract class IdentityService<TModel> : IUserService<TModel>
        where TModel : ApplicationUser
    {
        public abstract Task<TModel> AuthenticateUsingEmailAsync(string email, string password);
        public abstract Task<TModel> AuthenticateUsingUsernameAsync(string username, string password);
        public abstract Task<bool> CreateUserAsync(TModel user);
    }
}
