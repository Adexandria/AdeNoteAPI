using AdeAuth.Models;

namespace AdeAuth.Services.Interfaces
{
    /// <summary>
    /// Manages role services
    /// </summary>
    /// <typeparam name="TModel">Application role type</typeparam>
    public interface IRoleService<TModel> where TModel : ApplicationRole
    {
        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        Task<bool> CreateRolesAsync(List<TModel> roles);

        /// <summary>
        /// Creates role
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        Task<bool> CreateRoleAsync(TModel role);

        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
        Task<bool> DeleteRolesAsync(string[] roles);

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
        Task<bool> DeleteRoleAsync(string role);

        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        Task<bool> AddUserRoleAsync(Guid userId, string role);


        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        Task<bool> AddUserRoleAsync(string email, string role);

        /// <summary>
        /// Removes user role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        Task<bool> RemoveUserRoleAsync(Guid userId, string role);
    }
}
