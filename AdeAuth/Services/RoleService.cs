using AdeAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services
{
    internal class RoleService<TDbContext,TUser,TModel> : RoleManager<TModel>
        where TDbContext : DbContext
        where TUser : ApplicationUser
        where TModel : ApplicationRole
    {
        public RoleService(TDbContext dbContext)
        {
            _db = dbContext;
            _users = dbContext.Set<TUser>();
            _userRoles = dbContext.Set<UserRole>();
            _roles = dbContext.Set<TModel>();
        }
        public override async Task<bool> CreateRoleAsync(TModel role)
        {
            await _roles.AddAsync(role);

            return await SaveChangesAsync();
        }

        public override async Task<bool> CreateRolesAsync(List<TModel> roles)
        {
            foreach (var role in roles)
            {
               await _roles.AddAsync(role);
            }

            return await SaveChangesAsync();
        }

        public override async Task<bool> DeleteRoleAsync(string role)
        {
            var currentApplicationRole = await GetRole(role);
            if(currentApplicationRole != null)
            {
                _roles.Remove(currentApplicationRole);
                return await SaveChangesAsync();
            }
            return false;
        }

        public override async Task<bool> DeleteRolesAsync(string[] roles)
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

        public override async Task<bool> AddUserRole(Guid userId, string roleName)
        {
            var currentRole = await GetRole(roleName); 
            if (currentRole == null)
            {
                return false;
            }

            var currentUser = await _users.Where(s => s.Id == userId).FirstOrDefaultAsync();

            if (currentUser == null)
                return false;

            _userRoles.Add(new UserRole()
            {
                RoleId = currentRole.Id,
                UserId = currentUser.Id
            });

            return await SaveChangesAsync();
        }

        public override async Task<bool> RemoveUserRole(Guid userId, string roleName)
        {
            var currentRole = await GetRole(roleName);
            if (currentRole == null)
            {
                return false;
            }

            var currentUser = await _users.Where(s => s.Id == userId).FirstOrDefaultAsync();

            if (currentUser == null)
                return false;

            var userRole = await _userRoles.Where(s=>s.UserId == userId && s.RoleId ==  currentRole.Id).FirstOrDefaultAsync();

            _userRoles.Remove(userRole);
            
            return await SaveChangesAsync();
        }



        private async Task<TModel> GetRole(string roleName)
        {
            return await _roles.Where(s => s.Name == roleName)
                .FirstOrDefaultAsync();
        }

        private async Task<bool> SaveChangesAsync()
        {

            var saved = false;
            while (!saved)
            {
                try
                {
                    int commitedResult = await _db.SaveChangesAsync();
                    if (commitedResult == 0)
                    {
                        saved = false;
                        break;
                    }
                    saved = true;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    foreach (var entry in ex.Entries)
                    {
                        if (entry.Entity is TModel)
                        {
                            var proposedValues = entry.CurrentValues;
                            var databaseValues = entry.GetDatabaseValues();

                            foreach (var property in proposedValues.Properties)
                            {
                                var databaseValue = databaseValues[property];
                            }

                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
            }
            return saved;

        }


        private readonly DbSet<TUser> _users;
        private readonly DbSet<TModel> _roles;
        private readonly DbSet<UserRole> _userRoles;
        private readonly TDbContext _db;
    }

   
}
