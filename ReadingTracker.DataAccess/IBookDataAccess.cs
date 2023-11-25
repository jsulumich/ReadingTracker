using Microsoft.EntityFrameworkCore;
using ReadingTracker.DataAccess;

namespace ReadingTracker.Data
{
    public interface IBookDataAccess
    {
        public Task<IEnumerable<int>> GetDistinctYears();
        public Task<IEnumerable<Book>> GetBooksForYear(int year);
        public Task<Book> GetBookById(int? id);
        public Task<int> CreateBook(Book book);

        public Task<int> EditBook(Book book);
        public bool BookExists(int id);
        public Task<int> DeleteBook(int id);
        public Task<IEnumerable<Book>> SearchForBooks(string? Title, string? Author, DateTime? StartDate, DateTime? EndDate, string? Keyword);
    }

}
