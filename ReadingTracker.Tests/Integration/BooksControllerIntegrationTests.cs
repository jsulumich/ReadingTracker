using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ReadingTracker.Controllers;
using ReadingTracker.Domain;
using ReadingTracker.Tests.Integration;
using Xunit;

public class BooksControllerIntegrationTests :  IntegrationTestBase
{
    private readonly Mock<ILogger<BooksController>> _controllerLogger;
    private readonly BooksController _booksController;

    public BooksControllerIntegrationTests() : base()
    {
      
        _controllerLogger = new Mock<ILogger<BooksController>>();
        _booksController = new BooksController(_booksDataAccess, _controllerLogger.Object)
        {
            //need to mock TempData to avoid null reference exception
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            },
            TempData = new TempDataDictionary(new DefaultHttpContext(), Mock.Of<ITempDataProvider>())
        };
    }

    [Fact]
    public async Task Index_RetrievesBooksForCurrentYear_ByDefault()
    {
        // Arrange in base class
        
        // Act
        var result = await _booksController.Index(null);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var modelData = viewResult.Model as IEnumerable<Book>;
        Assert.NotNull(modelData);
        Assert.Equal(2, modelData.Count());

        IEnumerable<Book> expectedBooks = new List<Book>
            {
                base.GetBookById(1),
                base.GetBookById(2)
        }.OrderBy(book => book.StartDate);

        Assert.True(expectedBooks.SequenceEqual(modelData));
    }

    [Fact]
    public async Task Details_RetrieveBookById()
    {
        // Arrange in base class

        // Act
        var result = await _booksController.Details(1);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var modelData = Assert.IsAssignableFrom<Book>(viewResult.Model);
        Assert.Equal(modelData,base.GetBookById(1));
    }

    [Fact]
    public async Task Details_InvalidIdReturnsNotFound()
    {
        // Arrange in base class

        // Act
        var result = await _booksController.Details(4);

        // Assert
        Assert.IsType<NotFoundResult>(result);

    }


    [Fact]
    public async Task Create_AddsBookToDatabase()
    {
        // Arrange
        var book = new Book
        {
            Title = "Test Book",
            Author = "Test Author",
            StartDate = new DateTime(2023, 11, 01),
            EndDate = new DateTime(2023, 11, 25),
            PageCount = 200,
            Rating = 5
        };

        // Act
        var result = await _booksController.Create(book);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        var addedBook = _context.Books.FirstOrDefault(b => b.Title == "Test Book");
        Assert.NotNull(addedBook);
        Assert.Equal("Test Author", addedBook.Author);
        Assert.Equal(new DateTime(2023,11,01), addedBook.StartDate);
        Assert.Equal(new DateTime(2023, 11, 25), addedBook.EndDate);
        Assert.Equal(200, addedBook.PageCount);
        Assert.Equal(5, addedBook.Rating);
    
    }

    [Fact]
    public async Task ValidEdit_ReturnsViewResult()
    {
        //arrange in base class
        // Act
        var result = await _booksController.Edit(3);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task InvalidEdit_ReturnsNotFound()
    {
        //arrange in base class
        // Act
        var result = await _booksController.Edit(4);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ValidDelete_ReturnsView()
    {
        //arrange in base class
        // Act
        var result = await _booksController.Delete(2);

        // Assert
        Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task InvalidDelete_ReturnsNotFound()
    {
        //arrange in base class
        // Act
        var result = await _booksController.Delete(4);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteConfirmed_DeletesBook()
    {
        //arrange in base class
        // Act
        var result = await _booksController.DeleteConfirmed(2);

        // Assert
        var book = _context.Books.FirstOrDefault(b => b.Id == 2);
        Assert.Null(book);
    }

    [Fact]
    public async Task Edit_UpdatesBookRecord()
    {
        // Arrange
        var book = new Book
        {
            Id=3,
            Title = "Test Book",
            Author = "Test Author",
            StartDate = new DateTime(2023, 11, 01),
            EndDate = new DateTime(2023, 11, 25),
            PageCount = 200,
            Rating = 5
        };

       // Ensure the existing entity is not tracked
       var existingEntity = await _context.Books.FindAsync(3);
        if (existingEntity != null)
        {
            _context.Entry(existingEntity).State = EntityState.Detached;
        }

        // Act
        var result = await _booksController.Edit(3, book);

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);

        var editedBook = await _context.Books.FindAsync(3);

        Assert.NotNull(editedBook);
        Assert.Equal("Test Book", editedBook.Title);
        Assert.Equal("Test Author", editedBook.Author);
        Assert.Equal(new DateTime(2023, 11, 01), editedBook.StartDate);
        Assert.Equal(new DateTime(2023, 11, 25), editedBook.EndDate);
        Assert.Equal(200, editedBook.PageCount);
        Assert.Equal(5, editedBook.Rating);
    }

    // Dispose of resources specific to this class
    internal new void Dispose()
    {
           base.Dispose(); 
    }
 }
