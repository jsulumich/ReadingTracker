using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ReadingTracker.Controllers;
using Xunit;

namespace ReadingTracker.Tests.Unit
{
    public class SearchControllerUnitTests : UnitTestBase
    {
        private readonly Mock<ILogger<SearchController>> _controllerLogger;
        private readonly SearchController _searchController;

        public SearchControllerUnitTests() : base()
        {
            _controllerLogger = new Mock<ILogger<SearchController>>();
            _searchController = new SearchController(_mockBookDataAccess.Object, _controllerLogger.Object);
        }

        [Fact]
        public void Index_ReturnsViewResult()
        {
            // No Arrange

            // Act
            var result = _searchController.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }
    }

}



