using AdeAuth.Models;
using AdeAuth.Services.Interfaces;


namespace AdeAuth.Services
{
    /// <summary>
    /// Manages identity service for users
    /// </summary>
    /// <typeparam name="TModel">Application user</typeparam>
    public abstract class IdentityService<TModel> : IUserService<TModel>
        where TModel : ApplicationUser
    {

        /// <summary>
        /// Authenticate user using email and password
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public abstract Task<TModel> AuthenticateUsingEmailAsync(string email, string password);

        /// <summary>
        /// Authenticate user using username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public abstract Task<TModel> AuthenticateUsingUsernameAsync(string username, string password);

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user">New user to create</param>
        /// <returns>Boolean value</returns>
        public abstract Task<bool> CreateUserAsync(TModel user);
    }
}
