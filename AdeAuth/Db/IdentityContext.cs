using AdeAuth.Models;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Db
{

    public class IdentityContext : DbContext 
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<ApplicationRole> Roles { get; set; }

        public DbSet<ApplicationUser> Users { get; set; }
    }

    public class IdentityContext<TModel> : IdentityContext where TModel : ApplicationUser
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {
            
        }
    }

    public class IdentityContext<TUser,TRole> : IdentityContext 
        where TUser : ApplicationUser
        where TRole : ApplicationRole
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {

        }
    }
}
