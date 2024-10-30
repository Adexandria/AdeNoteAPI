using AdeAuth.Models;
using AdeAuth.Services.Interfaces;


namespace AdeAuth.Services
{
    /// <summary>
    /// Manages role services
    /// </summary>
    /// <typeparam name="TRole">Application role</typeparam>
    public abstract class RoleManager<TRole> : IRoleService<TRole>
        where TRole : ApplicationRole
    {
        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        public abstract Task<bool> AddUserRoleAsync(Guid userId, string role);


        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        public abstract Task<bool> AddUserRoleAsync(string email, string role);

        /// <summary>
        /// Creates role
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        public abstract Task<bool> CreateRoleAsync(TRole role);

        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        public abstract Task<bool> CreateRolesAsync(List<TRole> roles);

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        public abstract Task<bool> DeleteRoleAsync(string role);

        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        public abstract Task<bool> DeleteRolesAsync(string[] roles);

        /// <summary>
        /// Removes user role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        public abstract Task<bool> RemoveUserRoleAsync(Guid userId, string role);
    }
}
