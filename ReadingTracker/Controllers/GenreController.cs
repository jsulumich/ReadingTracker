using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Data;
using ReadingTracker.Domain;

namespace ReadingTracker.Controllers
{
    public class GenreController : Controller
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookDataAccess _bookDataAccess;

        public GenreController(IBookDataAccess bookDataAccess, ILogger<BooksController> logger)
        {
            _bookDataAccess = bookDataAccess;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return await TryCatch(async () =>
            {
                var genres = await _bookDataAccess.GetGenresAsync();

                return View(genres);
            });
        }
        public IActionResult Create()
        {
            var genre =  _bookDataAccess.GetEmptyGenre();
            return PartialView("_AddGenrePartialView", genre);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Genre genre)
        {
            return await TryCatch(async () =>
            {
                _logger.LogInformation("Adding genre:" + genre.Name);

                int result = await _bookDataAccess.CreateGenre(genre);

                if (result > 0)
                {
                    TempData["Message"] = "Added \"" + genre?.Name + "\" to database";
                }

                return RedirectToAction(nameof(Index));
            });
        }

        public async Task<IActionResult> Edit(int id)
        {
            return await TryCatch(async () =>
            {
                var genre = await _bookDataAccess.GetGenreById(id);
                return PartialView("_EditGenrePartialView", genre);
            });
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Genre genre)
        {
            return await TryCatch(async () =>
            {
                _logger.LogInformation("Editing genre:" + genre.Name);

                int result = await _bookDataAccess.EditGenre(genre);
                if (result > 0)
                {
                    TempData["Message"] = "Successfully edited \"" + genre.Name + "\"";
                    
                }
                _logger.LogInformation(result + "genre records updated");

                return RedirectToAction(nameof(Index));
            });
        }

        public async Task<IActionResult> Delete(int id)
        {
            return await TryCatch(async () =>
            {
                var genre = await _bookDataAccess.GetGenreById(id);
                return PartialView("_DeleteGenrePartialView", genre);
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            return await TryCatch(async () =>
            {              
                var genre = await _bookDataAccess.GetGenreById(id);
                _logger.LogInformation("Deleting genre:" + genre.Name);

                int result = await _bookDataAccess.DeleteGenre(id);
                if (result > 0)
                {
                    TempData["Message"] = "Deleted \"" + genre.Name + "\"";
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
