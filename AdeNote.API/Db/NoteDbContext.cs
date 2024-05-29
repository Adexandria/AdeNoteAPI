using AdeNote.Models;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<User> Users { get; set; }

        public DbSet<Ticket> Tickets { get; set; }

        /// <summary>
        /// Initialises the label page objects
        /// </summary>
        public DbSet<LabelPage> LabelPage { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public DbSet<HangfireUser> HangfireUsers {  get; set; } 


        /// <summary>
        /// An overriden method to handle the mapping of objects. \n
        /// It is also another way to set up the relationship or primary key
        /// </summary>
        /// <param name="modelBuilder">A property that is used to shape entities</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
                .HasOne(s=>s.User).WithMany(s=>s.Books).HasForeignKey(s=>s.UserId);

            modelBuilder.Entity<RefreshToken>()
                .HasOne(s => s.User).
                WithOne(s => s.RefreshToken).HasForeignKey("RefreshToken","UserId");

            modelBuilder.Entity<User>()
                .Property(s => s.Role)
                .HasDefaultValue(Role.User);

            modelBuilder.Entity<Book>()
                .HasMany(s => s.Pages);


            modelBuilder.Entity<Ticket>()
                .HasOne(s=>s.User)
                .WithMany(s => s.Tickets).HasForeignKey(s=>s.Issuer);



            modelBuilder.Entity<User>()
                .HasOne(s => s.RecoveryCode)
                .WithOne(s => s.User)
                .HasForeignKey("RecoveryCode", "UserId");


            modelBuilder.Entity<Page>()
                .HasMany(s => s.Labels)
                .WithMany(s => s.Pages)
                .UsingEntity<LabelPage>(
                l => l.HasOne<Label>().WithMany().HasForeignKey(s => s.LabelId),
                r=>r.HasOne<Page>().WithMany().HasForeignKey(s=>s.PageId));
        }
    }
}
