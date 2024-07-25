using AdeAuth.Models;

namespace AdeAuth.Services.Interfaces
{
    public interface IRoleService<TModel> where TModel : IApplicationRole
    {
        Task<bool> CreateRolesAsync(string[] roles);
        Task<bool> CreateRoleAsync(string role);
        Task<bool> DeleteRolesAsync(string[] roles);
        Task<bool> DeleteRoleAsync(string role);
    }
}
