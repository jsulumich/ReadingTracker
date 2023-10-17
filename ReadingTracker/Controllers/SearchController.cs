using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingTracker.Data;

namespace ReadingTracker.Controllers
{

    public class SearchController : Controller
    {

        private readonly ReadingTrackerDbContext _context;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ReadingTrackerDbContext context, ILogger<SearchController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string? Title, string? Author, DateTime? StartDate, DateTime? EndDate, string? Keyword)
        {
            _logger.LogInformation("Conducting search");

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

            return View(searchResults);
        }

    }
}
