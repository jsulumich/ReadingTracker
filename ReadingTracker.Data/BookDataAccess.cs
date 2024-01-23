using Microsoft.EntityFrameworkCore;
using ReadingTracker.Domain;
using Microsoft.Extensions.Logging;

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

        public async Task<IEnumerable<IBook>> GetBooksForYear(int year)
        {
            return await _context.Books
                .Where(book => book.StartDate.Year == year || book.EndDate.Year == year)
                    .OrderBy(book => book.StartDate)
                    .ToListAsync();
        }

        public async Task<IBook> GetBookById(int? id)
        {
            return await _context.Books
                .Include(b => b.Genre)
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync() ?? throw new Exception("Book not found");
        }

        public async Task<int> CreateBook(IBook book)
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

        public async Task<int> EditBook(IBook book)
        {
            _context.Update(book);
            return  await _context.SaveChangesAsync();
        }


        public bool BookExists(int id)
        { 
            return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        public async Task<IEnumerable<IBook>> SearchForBooks(string? Title, string? Author, DateTime? StartDate, DateTime? EndDate, string? Keyword)
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

        public async Task<IEnumerable<IGenre>> GetGenresAsync()
        {
            return await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<IGenre> GetGenreById(int? id)
        {
            return await _context.Genres
                .Where(g => g.Id == id)
                .FirstOrDefaultAsync() ?? throw new Exception("Genre not found");
        }

        public IGenre GetEmptyGenre()
        {
            return new Genre();
        }

        public async Task<int> CreateGenre(IGenre genre)
        {
            _context.Add(genre);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> EditGenre(IGenre genre)
        {
            _context.Update(genre);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteGenre(int id)
        {
            var genre = await _context.Genres.FindAsync(id);
            if (genre != null)
            {
                _context.Genres.Remove(genre);
            }

            return await _context.SaveChangesAsync();
        }



    }

}
