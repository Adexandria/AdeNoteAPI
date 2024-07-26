using AdeAuth.Models;

namespace AdeAuth.Services.Interfaces
{
    public interface IRoleService<TModel> where TModel : ApplicationRole
    {
        Task<bool> CreateRolesAsync(List<TModel> roles);
        Task<bool> CreateRoleAsync(TModel role);
        Task<bool> DeleteRolesAsync(string[] roles);
        Task<bool> DeleteRoleAsync(string role);
    }
}
