using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain;

namespace ReadingTracker.Data
{
    public class ReadingTrackerDbContext : DbContext
    {
        public ReadingTrackerDbContext(DbContextOptions<ReadingTrackerDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }


    }
}
