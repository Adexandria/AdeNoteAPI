using AdeAuth.Models;

namespace AdeAuth.Services.Interfaces
{
    public interface IRoleService<TModel> where TModel : ApplicationRole
    {
        Task<bool> CreateRolesAsync(List<TModel> roles);
        Task<bool> CreateRoleAsync(TModel role);
        Task<bool> DeleteRolesAsync(string[] roles);
        Task<bool> DeleteRoleAsync(string role);
        Task<bool> AddUserRole(Guid userId, string roleName);
        Task<bool> RemoveUserRole(Guid userId, string roleName);
    }
}
