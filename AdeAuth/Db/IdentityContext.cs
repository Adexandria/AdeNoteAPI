using AdeAuth.Models;
using Microsoft.EntityFrameworkCore;


namespace AdeAuth.Db
{

    public class IdentityContext : DbContext 
    {
        public IdentityContext(DbContextOptions options) : base(options)
        {

        }

        public  DbSet<ApplicationRole> Roles { get; set; }

        public  DbSet<ApplicationUser> Users { get; set; }

        public  DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationRole>()
                .HasMany<ApplicationUser>()
                .WithMany()
                .UsingEntity<UserRole>
                (l=> l.HasOne<ApplicationUser>().WithMany().HasForeignKey("UserId"),
                 r => r.HasOne<ApplicationRole>().WithMany().HasForeignKey("RoleId"));
        }
    }

    public class IdentityContext<TUser> : DbContext where TUser : ApplicationUser
    {
        public IdentityContext(DbContextOptions options): base(options)
        {
            
        }

        public  DbSet<TUser> Users { get; set; }

        public  DbSet<ApplicationRole> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationRole>()
                .HasMany<TUser>()
                .WithMany()
                .UsingEntity<UserRole>(l => l.HasOne<TUser>().WithMany().HasForeignKey("UserId"),
                 r => r.HasOne<ApplicationRole>().WithMany().HasForeignKey("RoleId"));
        }
    }

    public class IdentityContext<TUser,TRole> : DbContext
        where TUser : ApplicationUser
        where TRole : ApplicationRole
    {
        public IdentityContext(DbContextOptions options):base(options)
        {

        }
        public  DbSet<TUser> Users { get; set; }
        public  DbSet<TRole> Roles { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TRole>()
                .HasMany<TUser>()
                .WithMany()
                .UsingEntity<UserRole>(l => l.HasOne<TUser>().WithMany().HasForeignKey("UserId"),
                 r => r.HasOne<TRole>().WithMany().HasForeignKey("RoleId"));
        }
    }
}
