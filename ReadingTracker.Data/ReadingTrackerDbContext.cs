using Microsoft.EntityFrameworkCore;
namespace ReadingTracker.Data
{
    public class ReadingTrackerDbContext : DbContext
    {
        public ReadingTrackerDbContext(DbContextOptions<ReadingTrackerDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Genre> Genres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>()
                   .HasOne(b => b.Genre)
                   .WithMany()
                   .HasForeignKey(b => b.GenreId)
                   .OnDelete(DeleteBehavior.SetNull);

        }

    }
}
