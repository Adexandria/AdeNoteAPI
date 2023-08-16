using AdeNote.Models;
using Microsoft.EntityFrameworkCore;
using TasksLibrary.Models;

namespace AdeNote.Db
{
    /// <summary>
    /// Handles the transactions made to the database
    /// </summary>
    public class NoteDbContext : DbContext
    {
        /// <summary>
        /// A constructor
        /// </summary>
        /// <param name="options"></param>
        public NoteDbContext(DbContextOptions<NoteDbContext> options) : base(options)
        {

        }

        /// <summary>
        /// Initialises the label objects
        /// </summary>
        public DbSet<Label> Labels { get; set; }

        /// <summary>
        /// Initialises the book objects
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// Initialises the page objects
        /// </summary>
        public DbSet<Page> Pages { get; set; }

        /// <summary>
        /// Initialises the user objects
        /// </summary>
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// Initialises the label page objects
        /// </summary>
        public DbSet<LabelPage> LabelPage { get; set; }

        /// <summary>
        /// An overriden method to handle the mapping of objects. \n
        /// It is also another way to set up the relationship or primary key
        /// </summary>
        /// <param name="modelBuilder">A property that is used to shape entities</param>
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
