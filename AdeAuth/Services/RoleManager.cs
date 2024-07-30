using AdeAuth.Models;
using AdeAuth.Services.Interfaces;


namespace AdeAuth.Services
{
    public abstract class RoleManager<TRole> : IRoleService<TRole>
        where TRole : ApplicationRole
    {
        public abstract Task<bool> AddUserRole(Guid userId, string roleName);
        public abstract Task<bool> CreateRoleAsync(TRole role);
        public abstract Task<bool> CreateRolesAsync(List<TRole> roles);
        public abstract Task<bool> DeleteRoleAsync(string role);
        public abstract Task<bool> DeleteRolesAsync(string[] roles);
        public abstract Task<bool> RemoveUserRole(Guid userId, string roleName);
    }
}
