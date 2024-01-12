using Microsoft.AspNetCore.Mvc;
using ReadingTracker.Domain;

namespace ReadingTracker.Controllers
{
    [ApiController]
    // this uses the controller name for the route but I don't want that
    // I'll specify the route explicitly instead
    //[Route("api/[controller]")] 
    [Route("api/books")]
    public class ApiBooksController : ControllerBase
    {
        private readonly ILogger<BooksController> _logger;
        private readonly IBookDataAccess _bookDataAccess;

        public ApiBooksController(ILogger<BooksController> logger, IBookDataAccess bookDataAccess)
        {
            _logger = logger;
            _bookDataAccess = bookDataAccess;
        }


        // this is the default, but I like to be explicit
        [HttpGet]
        // IActionResult is a base class for all the other result types
        // allowing both a return value and a status code to be returned
        // we'll keep this generic and get more into details of various
        // IActionResult types later
        // again, "[FromQuery]" is the default but I like to be explicit
        public async Task<IActionResult> Get([FromQuery] int? year)
        {
            try
            {
                if (!year.HasValue)
                {
                    // Default to the current year
                    year = DateTime.Now.Year;
                }

                var booksForSelectedYear = 
                    await _bookDataAccess.GetBooksForYear(year.Value);

                // OK will convert the object to JSON and return a 200 status code
                // as JSON is the default format for WebAPIs
                // we'll get into other formats later
                return Ok(booksForSelectedYear);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
