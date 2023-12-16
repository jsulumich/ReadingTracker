using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingTracker.Data;
using ReadingTracker.Domain;

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
            try
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
            }
            catch(Exception  ex) 
            { 
                _logger.LogError(ex.Message);
                throw;
            }
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,StartDate,EndDate,PageCount,Rating")] Book book)
        {
            return await TryCatch(async () =>
            {
                if (ModelState.IsValid)
                {
                    _logger.LogInformation("Adding book:" + book.Title);
                    int result = await _bookDataAccess.CreateBook(book);
                    if (result > 0)
                    {
                        TempData["Message"] = "Added \"" + book.Title + "\" to database.";
                    }
                    return RedirectToAction(nameof(Index));
                }
                return View(book); ;
            });
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            return await TryCatch(async () =>
            {
                var book = await _bookDataAccess.GetBookById(id);

                return View(book);
            });
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,StartDate,EndDate,PageCount,Rating")] Book book)
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
                        int result = await _bookDataAccess.EditBook(book);
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


    }
}
