using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReadingTracker.Data;
using ReadingTracker.Models;

namespace ReadingTracker.Controllers
{
    public class BooksController : Controller
    {
        private readonly ReadingTrackerDbContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(ReadingTrackerDbContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Books
        public async Task<IActionResult> Index(int? year)
        {
            try
            {
                if (_context == null)
                {
                    return Problem("DbContext is null.");
                }

                var allBooks = await _context.Books.ToListAsync();

                // Get distinct years from both StartDate and EndDate
                var distinctYears = allBooks
                    .SelectMany(book => new[] { book.StartDate.Year, book.EndDate.Year })
                    .Distinct()
                    .ToList();

                ViewBag.distinctYears = distinctYears;

                if (!year.HasValue)
                {
                    // Default to the current year
                    year = DateTime.Now.Year;
                }
                ViewBag.selectedYear = year.Value;

                _logger.LogInformation("Fetched books read for " + year.Value.ToString()); 

                var books = allBooks
                    .Where(book => book.StartDate.Year == year || book.EndDate.Year == year)
                    .OrderBy(book => book.StartDate)
                    .ToList();

                return View(books);
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
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
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
            if (ModelState.IsValid)
            {
                _logger.LogInformation("Adding book:" + book.Title);
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,StartDate,EndDate,PageCount,Rating")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Editing Book:" + book.Title);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
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
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Books == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Books == null)
            {
                return Problem("Entity set 'ReadingTrackerDbContext.Books'  is null.");
            }
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }
            
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleting book: " + book?.Title);
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
          return (_context.Books?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
