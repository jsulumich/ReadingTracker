using ReadingTracker.Domain;

namespace ReadingTracker.Data;

public class Book : IBook
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? PageCount { get; set; }
    public int? Rating { get; set; }
}