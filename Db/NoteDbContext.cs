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
        public DbSet<LabelPage> LabelPage { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .Property(x => x.UserId)
                .HasColumnName("User_id");

            modelBuilder.Entity<User>()
                .Ignore("RefreshToken");

            modelBuilder.Entity<User>()
                .Ignore("AccessToken");

            modelBuilder.Entity<Book>()
                .HasMany(s => s.Pages);

            modelBuilder.Entity<Page>()
                .HasMany(s => s.Labels)
                .WithMany(s => s.Pages)
                .UsingEntity<LabelPage>(
                l => l.HasOne<Label>().WithMany().HasForeignKey(s => s.LabelId),
                r=>r.HasOne<Page>().WithMany().HasForeignKey(s=>s.PageId));


        }
    }
}
