using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Data;
using ReadingTracker.Domain;
using ReadingTracker.Models;

namespace ReadingTracker.Controllers
{
    public class StatisticController : Controller
    {
        private readonly ILogger<StatisticController> _logger;
        private readonly IBookDataAccess _bookDataAccess;
        private readonly BookStatisticsCalculator _bookStatsCalculator;

        public StatisticController(IBookDataAccess bookDataAccess, ILogger<StatisticController> logger,
            BookStatisticsCalculator bookStatsCalculator)
        {
            _bookDataAccess = bookDataAccess;
            _logger = logger;
            _bookStatsCalculator = bookStatsCalculator;
        }
        public async Task<IActionResult> Index(int? year)
        {
            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            // Get distinct years from both StartDate and EndDate
            var distinctYears = await _bookDataAccess.GetDistinctYears();
            ViewBag.selectedYear = year.Value;
            ViewBag.distinctYears = distinctYears; // move this to model
            _logger.LogInformation("Getting statistics for " + year.Value);
            IStatistic viewModel = await _bookStatsCalculator.GetStatisticsForYear(year.Value);

            return View(viewModel);
        }

    }
}
