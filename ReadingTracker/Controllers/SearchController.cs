using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Data;

namespace ReadingTracker.Controllers
{

    public class SearchController : Controller
    {

        private readonly ILogger<SearchController> _logger;
        private readonly IBookDataAccess _bookDataAccess;

        public SearchController(IBookDataAccess bookDataAccess, ILogger<SearchController> logger)
        {
            _bookDataAccess = bookDataAccess;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string? Title, string? Author, DateTime? StartDate, DateTime? EndDate, string? Keyword)
        {
            _logger.LogInformation("Conducting search");

            var searchResults = await _bookDataAccess.SearchForBooks(Title, Author, StartDate, EndDate, Keyword);

            return View(searchResults);
        }

    }
}
