using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingTracker.Data;
using ReadingTracker.Domain;
using ReadingTracker.Models;

namespace ReadingTracker.Controllers
{
    public class BooksController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookDataAccess _bookDataAccess;

        public BooksController(IBookDataAccess bookDataAccess, ILogger<BooksController> logger)
        {
            _bookDataAccess = bookDataAccess;
            _logger = logger;
        }

        // GET: Books
        public async Task<IActionResult> Index(int? year)
        {
            return await TryCatch(async () =>
            {
                // Get distinct years from both StartDate and EndDate
                var distinctYears = await _bookDataAccess.GetDistinctYears();

                ViewBag.distinctYears = distinctYears;

                if (!year.HasValue)
                {
                    // Default to the current year
                    year = DateTime.Now.Year;
                }
                ViewBag.selectedYear = year.Value;

                _logger.LogInformation("Fetched books read for " + year.Value);

                var booksForSelectedYear = await _bookDataAccess.GetBooksForYear(year.Value);

                return View(booksForSelectedYear);
            });
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            return await TryCatch(async () =>
            {
                if (id == null)
                {
                    return NotFound();
                }

                var book = await _bookDataAccess.GetBookById(id.Value);

                return View(book);
            });
        }

        // GET: Books/Create
        public async Task<IActionResult> Create()
        {
            return await TryCatch(async () =>
            {
                var viewModel = new BookWithGenreList
                {
                    Genres = await _bookDataAccess.GetGenresAsync()
                };

                return View(viewModel);
            });
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,StartDate,EndDate,PageCount,GenreId,Rating")] BookWithGenreList book)
        {
            return await TryCatch(async () =>
            {
                if (ModelState.IsValid)
                {
                    var bookToInsert = CreateBookForDatabase(book);

                    _logger.LogInformation("Adding book:" + book.Title);

                    int result = await _bookDataAccess.CreateBook(bookToInsert);
                    if (result > 0)
                    {
                        TempData["Message"] = "Added \"" + book.Title + "\" to database.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                book.Genres = await _bookDataAccess.GetGenresAsync();
                return View(book); 
            });
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await TryCatch(async () =>
            {
                var book = await _bookDataAccess.GetBookById(id);

                var viewModel = new BookWithGenreList
                {
                    Id = book.Id,
                    Title = book.Title,
                    Author = book.Author,
                    StartDate = book.StartDate,
                    EndDate = book.EndDate,
                    PageCount = book.PageCount,
                    Rating = book.Rating,
                    GenreId = book.GenreId,
                    Genres = await _bookDataAccess.GetGenresAsync()
                };

                return View(viewModel);
            });
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,StartDate,EndDate,PageCount,GenreId,Rating")] BookWithGenreList book)
        {
            return await TryCatch(async () =>
            {
                if (id != book.Id)
                {
                    return NotFound();
                }

                if (ModelState.IsValid)
                {
                    try
                    {
                        _logger.LogInformation("Editing Book:" + book.Title);

                        var bookToEdit = CreateBookForDatabase(book);
                        int result = await _bookDataAccess.EditBook(bookToEdit);
                        
                        if (result > 0)
                        {
                            TempData["Message"] = "Successfully edited \"" + book.Title + "\"";
                        }
                        
                        _logger.LogInformation(result + " book record updated");
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!_bookDataAccess.BookExists(book.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
                book.Genres = await _bookDataAccess.GetGenresAsync();
                return View(book);
            });
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            return await TryCatch(async () =>
            {
                var book = await _bookDataAccess.GetBookById(id);

                return View(book);
            });
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            return await TryCatch(async () =>
            {
                 var book = await _bookDataAccess.GetBookById(id);
                _logger.LogInformation("Deleting book: " + book?.Title);

                int result = await _bookDataAccess.DeleteBook(id);
                if (result > 0)
                {
                    TempData["Message"] = "Deleted \"" + book?.Title + "\" from database.";
                }
                return RedirectToAction(nameof(Index));
            });
        }

        private async Task<IActionResult> TryCatch(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private Book CreateBookForDatabase(BookWithGenreList bookWithGenre)
        {
            return new Book
            {
                Id = bookWithGenre.Id, 
                Title = bookWithGenre.Title,
                Author = bookWithGenre.Author,
                StartDate = bookWithGenre.StartDate,
                EndDate = bookWithGenre.EndDate,
                PageCount = bookWithGenre.PageCount,
                GenreId = bookWithGenre.GenreId,
                Rating = bookWithGenre.Rating
            };
        }


    }
}
