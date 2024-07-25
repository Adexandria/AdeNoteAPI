using AdeAuth.Models;


namespace AdeAuth.Services.Interfaces
{
    public interface IUserService<T> where T : IApplicationUser
    {
        public bool SignUpUser(T user);
        public bool AuthenticateUsingUsername(string username, string password);
        public bool AuthenticateUsingEmail(string email, string password);
        public bool AddRole(T User, string RoleName);
        public bool RemoveRole(T User, string RoleName);
    }
}
