using ReadingTracker.Domain;
using System.ComponentModel;

namespace ReadingTracker.Data;

public class Book : IBook
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Author { get; set; }
    [DisplayName("Start Date")]
    public DateTime StartDate { get; set; }
    [DisplayName("End Date")]
    public DateTime EndDate { get; set; }

    [DisplayName("Page Count")]
    public int? PageCount { get; set; }
    public int? Rating { get; set; }
    public int? GenreId { get; set; }
    public Genre? Genre { get; set; }

}