using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReadingTracker.Data;
using ReadingTracker.Domain;

namespace ReadingTracker.Tests.Integration
{

    public class IntegrationTestBase : IDisposable
    {
        protected readonly DbContextOptions<ReadingTrackerDbContext> _options;
        protected readonly Mock<ILogger<BookDataAccess>> _logger;
        protected readonly ReadingTrackerDbContext _context;
        protected readonly BookDataAccess _booksDataAccess;

        public IntegrationTestBase() 
        {

            // Set up the InMemory database provider for testing
            _options = new DbContextOptionsBuilder<ReadingTrackerDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ReadingTrackerDbContext(_options);
            _logger = new Mock<ILogger<BookDataAccess>>();
            _booksDataAccess = new BookDataAccess(_context, _logger.Object);

            var currentYear = DateTime.Now.Year;
            // Seed data
            _context.Books.AddRange(new Book
            {
                Title = "Book 1",
                Author = "Author 1",
                StartDate = new DateTime(currentYear, 1, 1),
                EndDate = new DateTime(currentYear, 1, 15),
                Rating = 4,
                PageCount = 300
            }, new Book
            {
                Title = "Book 2",
                Author = "Author 2",
                StartDate = new DateTime(currentYear, 2, 1),
                EndDate = new DateTime(currentYear, 2, 15),
                Rating = 5,
                PageCount = 400
            }, new Book
            {
                Title = "Book 3",
                Author = "Author 3",
                StartDate = new DateTime(2021, 1, 1),
                EndDate = new DateTime(2021, 1, 15)
            });

            _context.SaveChanges();

        }
        //helper method used in asserts
        protected Book GetBookById(int id)
        {
            var book = _context.Books.Find(id) ?? throw new Exception("Book not found");
            return book;
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    } 


}
