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

        private async Task<Statistic> GetStatsForYear(int? year)
        {
            if(year == null) { year = DateTime.Now.Year; }

            _logger.LogInformation("Getting statistics for " + year.Value);

            var statsForYear = new Statistic();

            var allBooksFromYear = await _bookDataAccess.GetBooksForYear(year.Value);

            statsForYear.TotalBooksRead = allBooksFromYear.Count();
           
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
