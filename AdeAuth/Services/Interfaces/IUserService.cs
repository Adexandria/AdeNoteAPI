using AdeAuth.Models;


namespace AdeAuth.Services.Interfaces
{
    public interface IUserService<TModel> where TModel : ApplicationUser
    {
        public Task<bool> SignUpUser(TModel user);
        public TModel AuthenticateUsingUsername(string username, string password);
        public TModel AuthenticateUsingEmail(string email, string password);
    }
}
