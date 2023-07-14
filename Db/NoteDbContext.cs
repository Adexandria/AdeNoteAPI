using AdeNote.Models;
using Microsoft.EntityFrameworkCore;
using TasksLibrary.Models;

namespace AdeNote.Db
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options)
        {

        }
        public DbSet<Label> Labels { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(x => x.UserId)
                .HasColumnName("User_id");

            modelBuilder.Entity<Book>()
                .HasMany(s => s.Pages);

            modelBuilder.Entity<Page>()
                .HasMany(s => s.Labels);

            modelBuilder.Entity<Label>()
                .HasMany(s=>s.Pages);

        }
    }
}
