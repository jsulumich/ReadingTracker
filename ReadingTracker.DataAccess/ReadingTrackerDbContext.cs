using Microsoft.EntityFrameworkCore;
using ReadingTracker.DataAccess;

namespace ReadingTracker.Data
{
    public class ReadingTrackerDbContext : DbContext
    {
        public ReadingTrackerDbContext(DbContextOptions<ReadingTrackerDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }


    }
}
