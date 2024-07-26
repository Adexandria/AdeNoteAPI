
using AdeAuth.Infrastructure;
using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Autofac;

namespace AdeAuth.Services
{
    /// <summary>
    /// Factory to manages password manager, token provider and muilti-factor services
    /// </summary>
    public class Application<TUser,TRole> 
        where TUser : ApplicationUser
        where TRole : ApplicationRole
    {
        /// <summary>
        /// Manages multi-factor service
        /// </summary>
        public IMfaService MfaService { get; set; }

        /// <summary>
        /// Manages password manager
        /// </summary>
        public IPasswordManager PasswordManager { get; set; }

        /// <summary>
        /// Manages token provider
        /// </summary>
        public ITokenProvider TokenProvider { get; set; }


        public IRoleService<TRole> RoleService { get; set; }

        public IUserService<TUser> UserService { get; set; }
    }
}
