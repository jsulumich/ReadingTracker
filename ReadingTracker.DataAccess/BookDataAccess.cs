using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ReadingTracker.DataAccess;

namespace ReadingTracker.Data
{
    public class BookDataAccess : IBookDataAccess
    {
        private readonly ReadingTrackerDbContext _context;
        private readonly ILogger<BookDataAccess> _logger;

        public BookDataAccess(ReadingTrackerDbContext context, ILogger<BookDataAccess> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task<IEnumerable<int>> GetDistinctYears()
        {

            var startYears = await _context.Books
               .Select(book => book.StartDate.Year)
               .Distinct()
               .ToListAsync();

            var endYears = await _context.Books
                .Select(book => book.EndDate.Year)
                .Distinct()
                .ToListAsync();
            
            var distinctYears = startYears.Concat(endYears).Distinct();

            return distinctYears;
        }

        public async Task<IEnumerable<Book>> GetBooksForYear(int year)
        {
            return await _context.Books
                .Where(book => book.StartDate.Year == year || book.EndDate.Year == year)
                    .OrderBy(book => book.StartDate)
                    .ToListAsync();
        }

        public async Task<Book> GetBookById(int? id)
        {
            return await _context.Books.FindAsync(id) ?? throw new Exception("Book not found");
        }

        public async Task<int> CreateBook(Book book)
        {
            _context.Add(book);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> EditBook(Book book)
        {
            _context.Update(book);
            return  await _context.SaveChangesAsync();
        }


        public bool BookExists(int id)
        { 
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IEnumerable<Book>> SearchForBooks(string? Title, string? Author, DateTime? StartDate, DateTime? EndDate, string? Keyword)
        {

            var query = _context.Books.AsQueryable();

            if (!string.IsNullOrEmpty(Title))
            {
                Title = Title.ToLower();
                query = query.Where(book => book.Title.ToLower().Contains(Title));
            }

            if (!string.IsNullOrEmpty(Author))
            {
                Author = Author.ToLower();
                query = query.Where(book => book.Author.ToLower().Contains(Author));
            }

            if (StartDate.HasValue)
            {
                query = query.Where(book => book.StartDate >= StartDate.Value.Date);
            }

            if (EndDate.HasValue)
            {
                query = query.Where(book => book.EndDate <= EndDate.Value.Date);
            }

            if (!string.IsNullOrEmpty(Keyword))
            {
                Keyword = Keyword.ToLower();
                query = query.Where(book => book.Title.ToLower().Contains(Keyword) || book.Author.ToLower().Contains(Keyword));
            }

            query = query.OrderBy(book => book.StartDate);

            var searchResults = await query.ToListAsync();

            return searchResults;
        }

    }

}
