using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Data;
using ReadingTracker.Models;

namespace ReadingTracker.Controllers
{
    public class StatisticController : Controller
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IBookDataAccess _bookDataAccess;

        public StatisticController(IBookDataAccess bookDataAccess, ILogger<StatisticController> logger)
        {
            _bookDataAccess = bookDataAccess;
            _logger = logger;   

        }
        public async Task<IActionResult> Index(int? year)
        {
            // Get distinct years from both StartDate and EndDate
            var distinctYears = await _bookDataAccess.GetDistinctYears();

            ViewBag.distinctYears = distinctYears;

            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }
            ViewBag.selectedYear = year.Value;

            var viewModel = await GetStatsForYear(year);

            return View(viewModel);
        }

        public async Task<Statistic> GetStatsForYear(int? year)
        {
            year ??= DateTime.Now.Year;

            _logger.LogInformation("Getting statistics for " + year.Value);

            var statsForYear = new Statistic();

            var allBooksFromYear = await _bookDataAccess.GetBooksForYear(year.Value);

            statsForYear.TotalBooksRead = allBooksFromYear.Count();

            statsForYear.AverageRating = allBooksFromYear
            .Where(book => book.Rating.HasValue)
            .Select(book => book.Rating.Value)
            .DefaultIfEmpty(0) // If there are no rated books, set the default value to 0
            .Average();

            statsForYear.TotalPagesRead = allBooksFromYear
            .Where(book => book.PageCount.HasValue)
            .Select(book => book.PageCount.Value)
            .DefaultIfEmpty(0) // If there are no page counts, set the default value to 0
            .Sum();

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
