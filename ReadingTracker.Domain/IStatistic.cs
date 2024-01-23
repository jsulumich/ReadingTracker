namespace ReadingTracker.Domain;

public interface IStatistic
{
    int? SelectedYear { get; set; }
    int? TotalBooksRead { get; set; }
    int? TotalPagesRead { get; set; }
    double? AverageRating { get; set; }
    double? AverageDaysPerBook { get; set; }
    string? TopAuthor { get; set; }
    Dictionary<string, Tuple<int, string>>? GenreBreakdown { get; set; }

}