using ReadingTracker.Domain;

namespace ReadingTracker.Data;

public class EfBookStatisticsCalculator : BookStatisticsCalculator
{
    private readonly IBookDataAccess _bookDataAccess;

    public EfBookStatisticsCalculator(IBookDataAccess bookDataAccess) : base(bookDataAccess)
    {
    }

    protected override IStatistic CreateStatistic(int year, int totalBooksRead, int? totalPagesRead, double? averageRating,
        double averageDaysPerBook, string topAuthor, Dictionary<string, Tuple<int, string>> genreBreakdown)
    {
        return new Statistic
        {
            SelectedYear = year,
            TotalBooksRead = totalBooksRead,
            TotalPagesRead = totalPagesRead,
            AverageRating = averageRating,
            AverageDaysPerBook = averageDaysPerBook,
            TopAuthor = topAuthor,
            GenreBreakdown = genreBreakdown
        };
    }
}