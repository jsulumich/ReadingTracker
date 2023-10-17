using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingTracker.Data;
using ReadingTracker.Models;

namespace ReadingTracker.Controllers
{
    public class StatisticController : Controller
    {
        private readonly ReadingTrackerDbContext _context;
        private readonly ILogger<StatisticController> _logger;
        public StatisticController(ReadingTrackerDbContext context, ILogger<StatisticController> logger)
        {
            _context = context;
            _logger = logger;   

        }
        public async Task<IActionResult> Index([FromQuery] int? year)
        {
            if (_context == null)
            {
                return Problem("DbContext is null.");
            }

            var allBooks = await _context.Books.ToListAsync();

            // Get distinct years from both StartDate and EndDate
            var distinctYears = allBooks
                .SelectMany(book => new[] { book.StartDate.Year, book.EndDate.Year })
                .Distinct()
                .ToList();

            ViewBag.distinctYears = distinctYears;

            if (!year.HasValue)
            {
                // Default to the current year
                year = DateTime.Now.Year;

            }
            ViewBag.selectedYear = year.Value;

            Statistic viewModel = GetStatsForYear(year);

            return View(viewModel);
        }

        private Statistic GetStatsForYear(int? year)
        {
            if(year == null) { year = DateTime.Now.Year; }

            _logger.LogInformation("Getting statistics for " + year.Value);

            var statsForYear = new Statistic();

            var allBooksFromYear = _context.Books
                .Where(book => book.StartDate.Year == year || book.EndDate.Year == year)
                .ToList();

            statsForYear.TotalBooksRead = allBooksFromYear.Count;
           
            statsForYear.AverageRating = allBooksFromYear
                .Where(book => book.Rating.HasValue)
                .Average(book => book.Rating.Value);

            statsForYear.TotalPagesRead = allBooksFromYear
                .Where(book => book.PageCount.HasValue)
                .Sum(book => book.PageCount.Value);

            statsForYear.TopAuthor = allBooksFromYear
                .GroupBy(book => book.Author)
                .OrderByDescending(group => group.Count())
                .Select(group => group.Key)
                .FirstOrDefault();

            double totalDays = allBooksFromYear
            .Sum(book => (book.EndDate - book.StartDate).TotalDays);

            statsForYear.AverageDaysPerBook = totalDays / statsForYear.TotalBooksRead;

            statsForYear.SelectedYear = year;

            return statsForYear;
        }
    }
}
