using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ReadingTracker.Controllers;
using ReadingTracker.Tests.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ReadingTracker.Tests.Integration
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
            var result =  _searchController.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }


        [Fact]
        public void Index_HandlesException()
        {
            // Arrange
            _mockBookDataAccess.Setup(repo => repo.GetDistinctYears()).Throws(new Exception("Simulated exception"));

            // Act and Assert
            Assert.Throws<Exception>(() => _searchController.Index());
        }

        [Fact]
        public void Index_SetsViewData()
        {
            // Act
            var result = _searchController.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);

            // Verify ViewData properties here, if applicable
            // Example: Assert.Equal(expectedValue, result.ViewData["SomeKey"]);
        }

        [Fact]
        public void Index_SetsViewBag()
        {
            // Act
            var result = _searchController.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);

            // Verify ViewBag properties here, if applicable
            // Example: Assert.Equal(expectedValue, result.ViewBag.SomeProperty);
        }
    }

    }



