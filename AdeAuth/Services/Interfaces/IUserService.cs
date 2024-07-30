using AdeAuth.Models;


namespace AdeAuth.Services.Interfaces
{
    public interface IUserService<TModel> where TModel : ApplicationUser
    {
        public Task<bool> CreateUserAsync(TModel user);
        public Task<TModel> AuthenticateUsingUsernameAsync(string username, string password);
        public Task<TModel> AuthenticateUsingEmailAsync(string email, string password);
    }
}
