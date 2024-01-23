using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ReadingTracker.Controllers;
using ReadingTracker.Domain;
using Xunit;

namespace ReadingTracker.Tests.Integration
{
    public class StatisticControllerIntegrationTests : IntegrationTestBase
    {
        private readonly Mock<ILogger<StatisticController>> _controllerLogger;
        private readonly StatisticController _statisticController;
        private readonly MockBookStatisticsCalculator _mockBookStatisticsCalculator;
        private readonly Mock<IStatistic> _mockStatistic;
        public StatisticControllerIntegrationTests() : base()
        {

            _controllerLogger = new Mock<ILogger<StatisticController>>();
            _mockStatistic = new Mock<IStatistic>();
            _mockBookStatisticsCalculator = new MockBookStatisticsCalculator(_booksDataAccess, _mockStatistic.Object);            
            _statisticController = new StatisticController(_booksDataAccess, _controllerLogger.Object, _mockBookStatisticsCalculator);

        }
        [Fact]
        public async Task Index_ReturnsViewWithModel()
        {
            // Arrange in base class
            
            // Act
            var result = await _statisticController.Index(null);

            // Assert
            Assert.IsType<ViewResult>(result);
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IStatistic>(viewResult.Model);
            Assert.Equal(_mockStatistic.Object, model);
        }


        [Fact]
        public async Task Index_ReturnsViewResult_WithCurrentYearSelected()
        {
            // Arrange
            var distinctYears = await _booksDataAccess.GetDistinctYears();

            // Act
            var result = await _statisticController.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("selectedYear"));
            Assert.Equal(DateTime.Now.Year, viewResult.ViewData["selectedYear"]);
            Assert.True(viewResult.ViewData.ContainsKey("distinctYears"));
            Assert.Equal(distinctYears, viewResult.ViewData["distinctYears"]);

            var model = viewResult.Model;
            Assert.NotNull(model);
            Assert.IsAssignableFrom<IStatistic>(model); 
            var stats = (IStatistic)model;
            Assert.NotNull(stats);
        }

   


    // Dispose of resources specific to this class
    internal new void Dispose()
        {
            base.Dispose();
        }
    }

    public class MockBookStatisticsCalculator : BookStatisticsCalculator
    {
        private readonly IStatistic _mockStatistic;

        public MockBookStatisticsCalculator(IBookDataAccess bookDataAccess, IStatistic mockStatistic)
            : base(bookDataAccess)
        {
            _mockStatistic = mockStatistic;
        }

        protected override IStatistic CreateStatistic(int year, int totalBooksRead, int? totalPagesRead,
            double? averageRating, double averageDaysPerBook, string topAuthor, Dictionary<string, Tuple<int, string>> genreBreakdown)
        {
            return _mockStatistic;
        }

    }

}
