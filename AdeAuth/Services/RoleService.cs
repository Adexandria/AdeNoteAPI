using AdeAuth.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeAuth.Services
{
    /// <summary>
    /// Manages role services
    /// </summary>
    /// <typeparam name="TRole">Application role</typeparam>
    public class RoleService<TDbContext,TUser,TModel> : RoleManager<TModel>
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

        /// <summary>
        /// Creates role
        /// </summary>
        /// <param name="role">New role to add</param>
        /// <returns>boolean value</returns>
        public override async Task<bool> CreateRoleAsync(TModel role)
        {
            await _roles.AddAsync(role);

            return await SaveChangesAsync();
        }


        /// <summary>
        /// Create roles
        /// </summary>
        /// <param name="roles">New roles to add</param>
        /// <returns>boolean value</returns>
        public override async Task<bool> CreateRolesAsync(List<TModel> roles)
        {
            foreach (var role in roles)
            {
               await _roles.AddAsync(role);
            }

            return await SaveChangesAsync();
        }


        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="role">Role to delete</param>
        /// <returns>Boolean value</returns>
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


        /// <summary>
        /// Delete roles
        /// </summary>
        /// <param name="roles">Roles to delete</param>
        /// <returns>Boolean value</returns>
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

        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        public override async Task<bool> AddUserRoleAsync(Guid userId, string role)
        {
            var currentRole = await GetRole(role); 
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

        /// <summary>
        /// Removes user role
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        public override async Task<bool> RemoveUserRoleAsync(Guid userId, string role)
        {
            var currentRole = await GetRole(role);
            if (currentRole == null)
            {
                return false;
            }

            var currentUser = await _users.Where(s => s.Id == userId).FirstOrDefaultAsync();

            if (currentUser == null)
                return false;

            var userRole = await _userRoles.Where(s=>s.UserId == userId && s.RoleId ==  currentRole.Id).FirstOrDefaultAsync();

            if(userRole == null)
            {
                return false; 
            }
            _userRoles.Remove(userRole);
            
            return await SaveChangesAsync();
        }


        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="email">User email address</param>
        /// <param name="role">Existing role name</param>
        /// <returns>Boolean value</returns>
        public override async Task<bool> AddUserRoleAsync(string email, string role)
        {
            var currentRole = await GetRole(role);
            if (currentRole == null)
            {
                return false;
            }

            var currentUser = await _users.Where(s => s.Email == email).FirstOrDefaultAsync();

            if (currentUser == null)
                return false;

            _userRoles.Add(new UserRole()
            {
                RoleId = currentRole.Id,
                UserId = currentUser.Id
            });

            return await SaveChangesAsync();
        }

        /// <summary>
        /// Get existing role
        /// </summary>
        /// <param name="role">Role</param>
        /// <returns>role</returns>
        private async Task<TModel> GetRole(string role)
        {
            return await _roles.Where(s => s.Name == role)
                .FirstOrDefaultAsync();
        }

        /// <summary>
        /// Save changes and manages concurrency exception
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
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
