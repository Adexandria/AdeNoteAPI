using AdeAuth.Db;
using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

namespace AdeNote.Db
{
    public class IdentityDbContext: IdentityContext<User>
    {
        public IdentityDbContext(DbContextOptions options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<RefreshToken>().ToTable("RefreshTokens");
            modelBuilder.Entity<RecoveryCode>().ToTable("RecoveryCodes");

            modelBuilder.Entity<User>().ToTable("Users");

            modelBuilder.Entity<RefreshToken>()
               .HasOne(s => s.User).
               WithOne(s => s.RefreshToken)
               .HasForeignKey("RefreshToken", "UserId");

            modelBuilder.Entity<User>()
             .HasOne(s => s.RecoveryCode)
             .WithOne(s => s.User)
             .HasForeignKey("RecoveryCode", "UserId");

            modelBuilder.Entity<User>()
               .Property(s => s.Role)
               .HasDefaultValue(Role.User);
        }

    }
}
