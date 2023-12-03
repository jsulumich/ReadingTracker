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
        var books = await _bookDataAccess.GetBooksForYear(year); //GetBooksForYear(year);
        

        return CreateStatistic(year, TotalBooksRead(books), 
            TotalPagesRead(books), 
            AverageRating(books), 
            AverageDaysPerBook(books), 
            TopAuthor(books));
    }
    protected abstract IStatistic CreateStatistic(int year, int totalBooksRead, int? totalPagesRead,
        double? averageRating, double averageDaysPerBook, string topAuthor);

    public static string TopAuthor(IEnumerable<IBook> books)
    {
        return books.GroupBy(book => book.Author)
            .OrderByDescending(group => group.Count())
            .First()
            .Key;
    }

    public static double AverageDaysPerBook(IEnumerable<IBook> books)
    {
        return books.Average(book => (book.EndDate - book.StartDate).TotalDays);
    }

    public static double? AverageRating(IEnumerable<IBook> books)
    {
        return books.Average(book => book.Rating);
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