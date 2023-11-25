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

        public StatisticControllerIntegrationTests() : base()
        {

            _controllerLogger = new Mock<ILogger<StatisticController>>();
            _statisticController = new StatisticController(_booksDataAccess, _controllerLogger.Object);

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
            Assert.IsAssignableFrom<IEnumerable<Statistic>>(model);
            var stats = (IEnumerable<Statistic>)model;

            Assert.NotNull(stats);
        }

        [Fact]
        public async Task GetStatsForYear_ReturnsExpectedResults()
        {
            //Arrange
            var currentYear = DateTime.Now.Year;

            //Act
            var result = await _statisticController.GetStatsForYear(currentYear);
            
            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalBooksRead); 
            Assert.Equal(4.5, result.AverageRating);
            Assert.Equal(700, result.TotalPagesRead);
            Assert.Equal(14, result.AverageDaysPerBook);
            Assert.Equal(currentYear, result.SelectedYear);
        }

        [Fact]
        public async Task GetStatsForYear_ReturnsExpectedResults_EmptyValues()
        {
            //Arrange
            var year = 2021;

            //Act
            var result = await _statisticController.GetStatsForYear(year);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalBooksRead);
            Assert.Equal(0, result.AverageRating);
            Assert.Equal(0, result.TotalPagesRead);
            Assert.Equal(14, result.AverageDaysPerBook);
            Assert.Equal(year, result.SelectedYear);
        }

        // Dispose of resources specific to this class
        internal new void Dispose()
        {
            base.Dispose();
        }
    }
 }
