namespace ReadingTracker.DataAccess
{
    public class Statistic
    {
        public int? SelectedYear { get; set; }
        public int? TotalBooksRead { get; set; }
        public int? TotalPagesRead { get; set; }
        public double? AverageRating { get; set; }
        public double? AverageDaysPerBook { get; set; }
        public string? TopAuthor { get; set; }
    }
}
