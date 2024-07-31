using AdeAuth.Models;


namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Manages user service
    /// </summary>
    /// <typeparam name="TModel">Application user</typeparam>
    public interface IUserService<TModel> where TModel : ApplicationUser
    {
        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user">New user to create</param>
        /// <returns>Boolean value</returns>
        public Task<bool> CreateUserAsync(TModel user);

        /// <summary>
        /// Authenticate user using username and password
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public Task<TModel> AuthenticateUsingUsernameAsync(string username, string password);

        /// <summary>
        /// Authenticate user using email and password
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="password">Password</param>
        /// <returns>User</returns>
        public Task<TModel> AuthenticateUsingEmailAsync(string email, string password);
    }
}
