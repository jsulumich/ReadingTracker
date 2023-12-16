using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ReadingTracker.Controllers;
using ReadingTracker.Domain;
using Xunit;

namespace ReadingTracker.Tests.Integration
{
    public class SearchControllerIntegrationTests : IntegrationTestBase
    {
        private readonly Mock<ILogger<SearchController>> _controllerLogger;
        private readonly SearchController _searchController;

        public SearchControllerIntegrationTests() : base()
        {
            _controllerLogger = new Mock<ILogger<SearchController>>();
            _searchController = new SearchController(_booksDataAccess, _controllerLogger.Object);

        }


        [Fact]
        public async Task SearchByYear_ReturnsExpectedResults()
        {
            //arrange 
            var startDate = new DateTime(2023, 1, 1);

            //act
            var result = await _searchController.Search(null, null, startDate, null, null);
            
            //assert
            var books = CommonAsserts(result);
            Assert.Equal(2, books.Count());

            IEnumerable<IBook> expectedBooks = new List<IBook>
            {
                base.GetBookById(1),
                base.GetBookById(2)
            }.OrderBy(book => book.StartDate);

            Assert.True(expectedBooks.SequenceEqual(books));
        }


        [Fact]
        public async Task SearchByExactTitle_ReturnsExpectedResults()
        {
            //arrange
            var title = "Book 3";

            //act
            var result = await _searchController.Search(title, null, null, null, null);
            
            //assert
            var books = CommonAsserts(result);
            Assert.Single(books);

            IEnumerable<IBook> expectedBooks = new List<IBook>
            {
                base.GetBookById(3),
            };

            Assert.True(expectedBooks.SequenceEqual(books));

        }

        [Fact]
        public async Task SearchByPartialTitle_ReturnsExpectedResults()
        {
            //arrange
            var title = "book";

            //act
            var result = await _searchController.Search(title, null, null, null, null);

            //assert
            var books = CommonAsserts(result);
            Assert.Equal(3, books.Count());

            IEnumerable<IBook> expectedBooks = new List<IBook>
            {
                base.GetBookById(1),
                base.GetBookById(2),
                base.GetBookById(3),
            }.OrderBy(book => book.StartDate);

            Assert.True(expectedBooks.SequenceEqual(books));

        }
        [Fact]
        public async Task SearchWithNoResults_ReturnsExpectedResults()
        {
            //arrange
            var title = "foobar";

            //act
            var result = await _searchController.Search(title, null, null, null, null);

            //assert
            var books = CommonAsserts(result);
            Assert.Empty(books);
        }

        private static IEnumerable<IBook> CommonAsserts(IActionResult result)
        {
            Assert.NotNull(result);

            var viewResult = Assert.IsType<ViewResult>(result);
            var model = viewResult.Model;

            Assert.NotNull(model);
            Assert.IsAssignableFrom<IEnumerable<IBook>>(model);

            return (IEnumerable<IBook>)model;
        }
        internal new void Dispose()
        {
            base.Dispose();
        }
    }




}
