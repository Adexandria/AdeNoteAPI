using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services
{
    internal class RoleService<TDbContext,TModel> : ServiceBase<TDbContext,ApplicationRole>, IRoleService<TModel>
        where TDbContext : DbContext
        where TModel : ApplicationRole
    {
        public RoleService(TDbContext dbContext) : base(dbContext)
        {
            _roles = dbContext.Set<TModel>();
        }
        public async Task<bool> CreateRoleAsync(TModel role)
        {
            await _roles.AddAsync(role);

            return await SaveChangesAsync();
        }

        public async Task<bool> CreateRolesAsync(List<TModel> roles)
        {
            foreach (var role in roles)
            {
               await _roles.AddAsync(role);
            }

            return await SaveChangesAsync();
        }

        public async Task<bool> DeleteRoleAsync(string role)
        {
            var currentApplicationRole = await GetRole(role);
            if(currentApplicationRole != null)
            {
                _roles.Remove(currentApplicationRole);
                return await SaveChangesAsync();
            }
            return false;
        }

        public async Task<bool> DeleteRolesAsync(string[] roles)
        {
            foreach (var role in roles)
            {
                var currentApplicationRole = await GetRole(role);
                if (currentApplicationRole == null)
                {
                    continue; 
                }
                _roles.Remove(currentApplicationRole);
            }
            return await SaveChangesAsync();
        }

        private async Task<TModel> GetRole(string roleName)
        {
            return await _roles.Where(s => s.Name == roleName).FirstOrDefaultAsync();
        }

        private DbSet<TModel> _roles;
    }

   
}
