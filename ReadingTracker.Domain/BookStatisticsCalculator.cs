namespace ReadingTracker.Domain;

public abstract class BookStatisticsCalculator
{
    private readonly IBookDataAccess _bookDataAccess;

    protected BookStatisticsCalculator(IBookDataAccess bookDataAccess)
    {
        _bookDataAccess = bookDataAccess;
    }

    public async Task<IStatistic> GetStatisticsForYear(int year)
    {
        var books = await _bookDataAccess.GetBooksForYear(year);
        var genres = await _bookDataAccess.GetGenresAsync();


        return CreateStatistic(year, TotalBooksRead(books),
            TotalPagesRead(books),
            AverageRating(books),
            AverageDaysPerBook(books),
            TopAuthor(books),
            GenreBreakdown(books, genres)
            );
    }

    protected abstract IStatistic CreateStatistic(int year, int totalBooksRead, int? totalPagesRead,
    double? averageRating, double averageDaysPerBook, string topAuthor, Dictionary<string, Tuple<int, string>> genreBreakdown);


    private Dictionary<string, Tuple<int, string>> GenreBreakdown(IEnumerable<IBook> books, IEnumerable<IGenre> genres)
    {

        var genreBreakdown = new Dictionary<string, Tuple<int, string>>();

        if (books.Any())
        {

            var genreCounts = books.GroupBy(book => book.GenreId)
                                   .ToDictionary(group => group.Key ?? 0, group => group.Count());

            var totalBooks = books.Count();

            foreach (var genreCount in genreCounts)
            {
                var genreName = genres.FirstOrDefault(g => g.Id == genreCount.Key)?.Name ?? "Not Specified";
                var color = genres.FirstOrDefault(g => g.Id == genreCount.Key)?.Color ?? "#808080";
                var percentage = (int)Math.Round((double)genreCount.Value / totalBooks * 100);

                genreBreakdown.Add(genreName, new Tuple<int, string>(percentage, color));
            }
        }

        return genreBreakdown;
    }

    public static string TopAuthor(IEnumerable<IBook> books)
    {
        return books.Any() ? 
            books.GroupBy(book => book.Author)
            .OrderByDescending(group => group.Count())
            .First()
            .Key : "N/A";
    }

    public static double AverageDaysPerBook(IEnumerable<IBook> books)
    {
        return books.Any() ? books.Average(book => (book.EndDate - book.StartDate).TotalDays) : 0;
    }

    public static double? AverageRating(IEnumerable<IBook> books)
    {
        return books.Any() ? books.Average(book => book.Rating) : 0;
    }

    public static int? TotalPagesRead(IEnumerable<IBook> books)
    {
        return books.Sum(book => book.PageCount);
    }

    public static int TotalBooksRead(IEnumerable<IBook> books)
    {
        return books.Count();
    }
}