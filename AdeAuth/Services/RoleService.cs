using AdeAuth.Models;
using AdeAuth.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services
{
    internal class RoleService<TDbContext> : ServiceBase<TDbContext,ApplicationRole>, IRoleService<ApplicationRole>
        where TDbContext : DbContext
    {
        public RoleService(TDbContext dbContext) : base(dbContext)
        {
            _roles = dbContext.Set<ApplicationRole>();
        }
        public async Task<bool> CreateRoleAsync(string role)
        {
            var newRole = new ApplicationRole(role);

            await _roles.AddAsync(newRole);

            return await SaveChangesAsync();
        }

        public async Task<bool> CreateRolesAsync(string[] roles)
        {
            foreach (var role in roles)
            {
               await _roles.AddAsync(new ApplicationRole(role));
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

        private async Task<ApplicationRole> GetRole(string roleName)
        {
            return await _roles.Where(s => s.Name == roleName).FirstOrDefaultAsync();
        }

        private DbSet<ApplicationRole> _roles;
    }

   
}
