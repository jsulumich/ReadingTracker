using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ReadingTracker.Controllers;
using Xunit;

namespace ReadingTracker.Tests.Unit
{
    public class BooksControllerUnitTests : UnitTestBase
    {
        private readonly BooksController _controller;
        private readonly Mock<ILogger<BooksController>> _mockLogger;
        public BooksControllerUnitTests() : base()
        {
            _mockLogger = new Mock<ILogger<BooksController>>();
            _controller =  new BooksController(_mockBookDataAccess.Object, _mockLogger.Object);

        }

        [Fact]
        public async Task Index_ReturnsViewResult_WithCurrentYearSelected()
        {
            // Arrange
            var distinctYears = new List<int> {2022, 2023, 2024 };
            _mockBookDataAccess.Setup(repo => repo.GetDistinctYears())
                .ReturnsAsync(distinctYears);

            // Act
            var result = await _controller.Index(null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.True(viewResult.ViewData.ContainsKey("selectedYear")); 
            Assert.Equal(DateTime.Now.Year, viewResult.ViewData["selectedYear"]);
            Assert.True(viewResult.ViewData.ContainsKey("distinctYears"));
            Assert.Equal(distinctYears, viewResult.ViewData["distinctYears"]);

        }

        [Fact]
        public void Create_ReturnsViewResult()
        {
            // Arrange in constructor
            
            // Act
            var result = _controller.Create();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

    }
}
